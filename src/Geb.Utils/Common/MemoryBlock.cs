using System;
using System.Runtime.InteropServices;

namespace Geb.Utils
{
    using SizeT = System.Int32;

    #region MemoryBlock

    /********************************************************************
     * 
     * 这是一个内存管理类，用于管理MemoryStorge。由OpenCV的相关实现修改而来。
     * 
     ********************************************************************/

    public unsafe struct MemBlock
    {
        public MemBlock* Prev;
        public MemBlock* Next;
    }

    public unsafe struct MemStorage
    {
        public int Signature;
        public int BlockSize;               /* Block size.                              */
        public int FreeSpace;               /* Remaining free space in current block.   */
        public MemBlock* Bottom;           /* First allocated block.                   */
        public MemBlock* Top;              /* Current memory block - top of the stack. */
        public MemStorage* Parent;         /* We get new blocks from parent as needed. */
    }

    public unsafe struct MemStoragePos
    {
        public MemBlock* Top;
        public int FreeSpace;
    }

    public unsafe class MemoryBlock
    {
        private int BlockSize;
        private int FreeSpace;
        private MemStorage* Storage;

        public static void Create(MemStorage* storage, int blockSize)
        {
            if (blockSize <= 0) throw new ArgumentException("blockSize must > 0.");

            blockSize = Align(blockSize, 8);
            storage = (MemStorage*)Marshal.AllocHGlobal(sizeof(MemStorage));
            storage->BlockSize = blockSize;
            Std.Memset(storage, 0, sizeof(MemStorage));
        }

        public static void ClearMemStorage(MemStorage* storage)
        {
            if (storage == null) return;

            if (storage->Parent != null)
                DestroyMemStorage(storage);
            else
            {
                storage->Top = storage->Bottom;
                storage->FreeSpace = (storage->Bottom != null) ? storage->BlockSize - sizeof(MemBlock) : 0;
            }
        }

        public static void DestroyMemStorage(MemStorage* storage)
        {
            int k = 0;

            MemBlock* block;
            MemBlock* dst_top = null;

            if (storage == null) return;

            if (storage->Parent != null)
                dst_top = storage->Parent->Top;

            for (block = storage->Bottom; block != null; k++)
            {
                MemBlock* temp = block;

                block = block->Next;
                if (storage->Parent != null)
                {
                    if (dst_top != null)
                    {
                        temp->Prev = dst_top;
                        temp->Next = dst_top->Next;
                        if (temp->Next != null)
                            temp->Next->Prev = temp;
                        dst_top = dst_top->Next = temp;
                    }
                    else
                    {
                        dst_top = storage->Parent->Bottom = storage->Parent->Top = temp;
                        temp->Prev = temp->Next = null;
                        storage->FreeSpace = storage->BlockSize - sizeof(MemBlock);
                    }
                }
                else
                {
                    Marshal.FreeHGlobal((IntPtr)temp);
                }
            }

            storage->Top = storage->Bottom = null;
            storage->FreeSpace = 0;
        }

        public static void ReleaseMemStorage(MemStorage** storage)
        {
            if (storage == null) return;

            MemStorage* st = *storage;
            *storage = null;
            if (st != null)
            {
                DestroyMemStorage(st);
                Marshal.FreeHGlobal((IntPtr)st);
            }
        }

        public static int Align(int size, int align)
        {
            return (size + align - 1) & -align;
        }

        public static int AlignLeft(int size, int align)
        {
            return size & -align;
        }

        public static void* Alloc(MemStorage* storage, int size)
        {
            Byte* ptr = null;

            if (storage == null) return null;

            if (size > Int32.MaxValue) return null;

            if ((SizeT)storage->FreeSpace < size)
            {
                SizeT max_free_space = AlignLeft(storage->BlockSize - sizeof(MemBlock), 8);
                if (max_free_space < size) return null;

                GoNextMemBlock(storage);
            }

            ptr = ((Byte*)(storage)->Top + (storage)->BlockSize - (storage)->FreeSpace);
            storage->FreeSpace = AlignLeft(storage->FreeSpace - (int)size, 8);

            return ptr;
        }

        /* Remember memory storage position: */
        public static void SaveMemStoragePos(MemStorage* storage, MemStoragePos* pos)
        {
            if (storage == null || pos == null) return;

            pos->Top = storage->Top;
            pos->FreeSpace = storage->FreeSpace;
        }


        /// <summary>
        /// Restore memory storage position
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="pos"></param>
        public static void RestoreMemStoragePos(MemStorage* storage, MemStoragePos* pos)
        {
            if (storage == null || pos == null) return;

            if (pos->FreeSpace > storage->BlockSize) throw new Exception("bad size");

            storage->Top = pos->Top;
            storage->FreeSpace = pos->FreeSpace;

            if (storage->Top == null)
            {
                storage->Top = storage->Bottom;
                storage->FreeSpace = (storage->Top != null) ? storage->BlockSize - sizeof(MemBlock) : 0;
            }
        }

        /// <summary>
        /// Moves stack pointer to next block. 
        /// If no blocks, allocate new one and link it to the storage.
        /// </summary>
        /// <param name="storage"></param>
        public static void GoNextMemBlock(MemStorage* storage)
        {
            if (storage == null) return;

            if (storage->Top == null || storage->Top->Next == null)
            {
                MemBlock* block;

                if ((storage->Parent) == null)
                {
                    block = (MemBlock*)Marshal.AllocHGlobal(storage->BlockSize);
                }
                else
                {
                    MemStorage* parent = storage->Parent;
                    MemStoragePos parent_pos;

                    SaveMemStoragePos(parent, &parent_pos);
                    GoNextMemBlock(parent);

                    block = parent->Top;
                    RestoreMemStoragePos(parent, &parent_pos);

                    if (block == parent->Top)  /* the single allocated block */
                    {
                        parent->Top = parent->Bottom = null;
                        parent->FreeSpace = 0;
                    }
                    else
                    {
                        /* cut the block from the parent's list of blocks */
                        parent->Top->Next = block->Next;
                        if (block->Next != null)
                            block->Next->Prev = parent->Top;
                    }
                }

                /* link block */
                block->Next = null;
                block->Prev = storage->Top;

                if (storage->Top != null)
                    storage->Top->Next = block;
                else
                    storage->Top = storage->Bottom = block;
            }

            if (storage->Top->Next != null)
                storage->Top = storage->Top->Next;
            storage->FreeSpace = storage->BlockSize - sizeof(MemBlock);
        }
    }

    
    #endregion
}
