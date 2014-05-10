using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace Perfect_Dark_Automation {
    public static class Memory {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("Kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
                                                    byte[] lpBuffer, UInt32 nSize, ref UInt32 lpNumberOfBytesRead);

        public static int[] tableBounds = new int[] { 0x0D090010, 0x1FFFFFFF };
        public static int[] tableLocs = new int[4];
        public static uint itemSize = 0x2D8;
        public static Table SearchTable, DownloadCompleteTable;
        public static IntPtr handle;
        public static Process process;

        public static bool Update(bool fromAuto) {
            if (CheckHandle()) {
                if (tableLocs[0] == 0) {
                    GetTableLocs(process.Handle);
                }
                if (!fromAuto)
                    Log.WriteLine("Memory - Forced Update");
                if (UI.Update()) {
                    UpdateSearch();
                    UpdateDownload(); // DO DATA GETS
                    UpdateComplete();
                    return true;
                }
            }
            else {
                Log.WriteLine("Unable to find Perfect Dark process...");
            }
            return false;
        }

        public static bool CheckHandle() {
            Process[] localByName = Process.GetProcessesByName("perfect dark");
            if (localByName.Length > 0) {
                process = localByName[0];
                if (process.Handle == IntPtr.Zero)
                    UI.getNew = true;
                return true;
            }
            else {
                tableLocs = new int[4];
                UI.getNew = true;
                return false;
            }
        }

        public static byte[] Read(IntPtr handle, IntPtr address, UInt32 size) {
            byte[] buffer = new byte[size];
            uint tmp = new uint();
            ReadProcessMemory(handle, address, buffer, size, ref tmp);
            return buffer;
        }

        public static void GetTableLocs(IntPtr perfectDark) {
            SearchTable = new Table();
            DownloadCompleteTable = new Table();
            int bytesRead = 0;
            byte[] buffer = new byte[1024];
            byte[] memory = new byte[(tableBounds[1] - tableBounds[0]) + 1024];
            byte[] tableStart = { 0x40, 0x16, 0x00, 0x00, 0x40, 0x16 };
            while (tableBounds[0] + bytesRead < tableBounds[1]) {
                buffer = Read(perfectDark, (IntPtr)tableBounds[0] + bytesRead, 1024);
                buffer.CopyTo(memory, bytesRead);
                bytesRead += buffer.Length;
            }
            int[] offsets = ByteSearch(memory, tableStart, 4);
            memory = null; // BECAUSE FUCKING LAZY
            GC.Collect(); // BECAUSE FUCKING LAZY
            //File.WriteAllBytes("D:\\derp.derp", memory); // DUMP MEMORY TO FILE
            for (int t = 0; t < offsets.Length; t++) {
                tableLocs[t] = tableBounds[0] + offsets[t] + 0xF;
                byte[] small = Read(process.Handle, (IntPtr)tableLocs[t], 32);
            }

            byte[] firstItem = Read(process.Handle, (IntPtr)(tableLocs[1]), 32);
        }

        static public int[] ByteSearch(byte[] arrayToSearch, byte[] input, int max) {
            int[] indexes = new int[max];
            int counter = 0;
            int currentIndex = -1;
            for (int t = 0; t < arrayToSearch.Length; t++) {
                if (arrayToSearch[t] == input[0]) {
                    currentIndex = t;
                    for (int p = 1; p < input.Length; p++) {
                        if (arrayToSearch[t + p] == input[p]) {
                            if (p == input.Length - 1) {
                                if (counter == max) {
                                    t = arrayToSearch.Length + 1;
                                }
                                else {
                                    indexes[counter] = currentIndex;
                                    counter++;
                                }
                                break;
                            }
                        }
                        else {
                            currentIndex = -1;
                            break;
                        }
                    }
                }
            }
            return indexes;
        }

        static public Item ItemFromByteArray(byte[] byteArray, bool isSearchPane, bool isDownloadPane) {
            if (byteArray[0x30] == 0x1) {
                byte[] hash = new byte[32];
                byte[] rightWayHash = new byte[32];
                byte[] singleByte = new byte[1];
                Item tmpItem = new Item();
                //0x8 - 32 Bytes - hash BACKWARDS byte by byte 
                Array.Copy(byteArray, 8, hash, 0, 32);
                for (int t = 0; t < hash.Length; t++) {
                    rightWayHash[t] = hash[hash.Length - 1 - t];
                }
                tmpItem.hash = BitConverter.ToString(rightWayHash).Replace("-", string.Empty);
                tmpItem.hash = tmpItem.hash.ToLower();
                //0x28 - Byte - Selection,  Not Selected = 0; Selected = 1; Mouse Down = 2;
                Array.Copy(byteArray, 0x28, singleByte, 0, 1);
                if (singleByte[0] == 0x0)
                    tmpItem.selected = false;
                else
                    tmpItem.selected = true;

                //0x34 - Byte - Status, (in search:0 = new; 1 = downloading;? > 1 =  done;); 1 = downloading; 2 = completed(?)
                Array.Copy(byteArray, 0x34, singleByte, 0, 1);
                if (singleByte[0] == 0x00)
                    tmpItem.hasBeenDownloaded = false;
                else
                    tmpItem.hasBeenDownloaded = true;

                //0x35 - Byte - Face, 0 = Sleeping; 1 = Converting; 2 = Working; - ONLY ON DOWNLOAD
                if (!isSearchPane) {
                    if (isDownloadPane) {
                        tmpItem.isDownload = true;
                        Array.Copy(byteArray, 0x35, singleByte, 0, 1);
                        tmpItem.face = singleByte[0];
                    }
                    else
                        tmpItem.isComplete = true;
                }
                else {
                    tmpItem.isSearch = true;
                }

                //0x38 - 4 Bytes - Filename /w tags length
                byte[] intArray = new byte[4];
                Array.Copy(byteArray, 0x38, intArray, 0, 4);
                int fileNameLengthWithTags = BitConverter.ToInt32(intArray, 0) * 2;
                //0x3C - 4 Bytes - Filename /wo tags length
                Array.Copy(byteArray, 0x3C, intArray, 0, 4);
                int fileNameLengthWithoutTags = BitConverter.ToInt32(intArray, 0) * 2;
                //0x50 - Variable String UNICODE - Start of Filename
                byte[] fileName = new byte[fileNameLengthWithoutTags];
                Array.Copy(byteArray, 0x50, fileName, 0, fileNameLengthWithoutTags);
                tmpItem.fileName = Encoding.Unicode.GetString(fileName);
                byte[] fileNameWithTags = new byte[fileNameLengthWithTags];
                Array.Copy(byteArray, 0x50, fileNameWithTags, 0, fileNameLengthWithTags);
                string fileNameWithTagsEncoded = Encoding.Unicode.GetString(fileNameWithTags);

                string tags = fileNameWithTagsEncoded.Substring(fileNameLengthWithoutTags / 2);
                string[] tagsArray = tags.Split(new string[] { "\0" }, 10, StringSplitOptions.RemoveEmptyEntries);
                //GETTING UPLOADER
                if (tagsArray[tagsArray.Length - 1] == "@") {
                    byte[] uploaderMem = new byte[itemSize - 0x50 - fileNameLengthWithTags];
                    Array.Copy(byteArray, 0x50 + fileNameLengthWithTags, uploaderMem, 0, uploaderMem.Length - 1);
                    int length = ByteSearch(uploaderMem, new byte[] { 0x00, 0x00 }, 1)[0] - 1;
                    tmpItem.uploader = Encoding.Unicode.GetString(uploaderMem, 0, length);
                    tmpItem.tags = new string[tagsArray.Length - 1];
                    Array.Copy(tagsArray, tmpItem.tags, tagsArray.Length - 1);
                }
                else {
                    tmpItem.tags = tagsArray;
                }

                //0x258 - 4 Bytes - Size in bytes
                Array.Copy(byteArray, 0x258, intArray, 0, 4);
                tmpItem.fileSize = BitConverter.ToInt32(intArray, 0);
                //0x268 - 4 Bytes - Count
                Array.Copy(byteArray, 0x268, intArray, 0, 4);
                tmpItem.count = BitConverter.ToInt32(intArray, 0);
                //0x2A8 - Float - Progress percentage
                Array.Copy(byteArray, 0x2A8, intArray, 0, 4);
                tmpItem.progress = BitConverter.ToSingle(intArray, 0);
                return tmpItem;
            }
            else {
                return null;
            }



        }

        static public void UpdateSearch() {
            UI.SwitchToSearch();
            byte[] block = Read(process.Handle, (IntPtr)tableLocs[0], itemSize); //0x0 - Table Item Start
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            SearchTable.SetAllDisposable();
            while (counter > -1) {
                Item tmpItem = ItemFromByteArray(block, true, false);
                if (tmpItem != null) {
                    int tmpIndex = SearchTable.ContainsItemByHash(tmpItem.hash);
                    if (tmpIndex == -1) {
                        SearchTable.Add(tmpItem); // ITS A NEW ITEM 

                    }
                    else
                        SearchTable.GetItemByIndex(tmpIndex).doDispose = false; //ADD UPDATING CODE LATER HERE? MAKE IT NOT DISPOSE
                    block = Read(process.Handle, (IntPtr)(tableLocs[0] + (itemSize * counter)), itemSize);
                    counter++;
                }
                else {
                    sw.Stop();
                    long past = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L));
                    SearchTable.CheckForDisposable();
                    counter = -1;
                }
            }
            if (!Filters.checking)
                Filters.Check();
        }

        static public void UpdateDownload() {
            UI.SwitchToDownload();
            byte[] block = Read(process.Handle, (IntPtr)tableLocs[1], itemSize); //0x0 - Table Item Start
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (counter > -1) {
                Item tmpItem = ItemFromByteArray(block, false, true);
                if (tmpItem != null) {
                    if (DownloadCompleteTable.ContainsItemByHash(tmpItem.hash) == -1)
                        DownloadCompleteTable.Add(tmpItem);
                    block = Read(process.Handle, (IntPtr)(tableLocs[1] + (itemSize * counter)), itemSize);
                    counter++;
                }
                else {
                    sw.Stop();
                    long past = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L));
                    counter = -1;
                }
            }
        }

        static public void UpdateComplete() {
            UI.SwitchToComplete();
            byte[] block = Read(process.Handle, (IntPtr)tableLocs[1], itemSize); //0x0 - Table Item Start
            int counter = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (counter > -1) {
                Item tmpItem = ItemFromByteArray(block, false, true);
                if (tmpItem != null) {
                    if (DownloadCompleteTable.ContainsItemByHash(tmpItem.hash) == -1)
                        DownloadCompleteTable.Add(tmpItem);
                    block = Read(process.Handle, (IntPtr)(tableLocs[1] + (itemSize * counter)), itemSize);
                    counter++;
                }
                else {
                    sw.Stop();
                    long past = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L));
                    counter = -1;
                }
            }
        }
    }
}
