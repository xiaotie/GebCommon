using System;
using System.Runtime.InteropServices;

namespace Geb.Utils
{
    using SizeT = System.Int32;

    public unsafe struct CvMemBlock
    {
        public CvMemBlock* prev;
        public CvMemBlock* next;
    }

    public unsafe struct CvMemStorage
    {
        public int signature;
        public CvMemBlock* bottom;           /* First allocated block.                   */
        public CvMemBlock* top;              /* Current memory block - top of the stack. */
        public CvMemStorage* parent;         /* We get new blocks from parent as needed. */
        public int block_size;               /* Block size.                              */
        public int free_space;               /* Remaining free space in current block.   */
    }

    public unsafe struct CvMemStoragePos
    {
        public CvMemBlock* top;
        public int free_space;
    }

    public unsafe class MemoryBlock
    {
        private int BlockSize;
        private int FreeSpace;
        private CvMemStorage* storage;

        public static void Create(CvMemStorage* storage, int blockSize)
        {
            if (blockSize <= 0) throw new ArgumentException("blockSize must > 0.");

            blockSize = Align(blockSize, 8);
            storage = (CvMemStorage*)Marshal.AllocHGlobal(sizeof(CvMemStorage));
            storage->block_size = blockSize;
            Std.Memset(storage, 0, sizeof(CvMemStorage));
        }

        public static void cvClearMemStorage(CvMemStorage* storage)
        {
            if (storage == null) return;

            if (storage->parent != null)
                icvDestroyMemStorage(storage);
            else
            {
                storage->top = storage->bottom;
                storage->free_space = (storage->bottom != null) ? storage->block_size - sizeof(CvMemBlock) : 0;
            }
        }

        public static void icvDestroyMemStorage(CvMemStorage* storage)
        {
            int k = 0;

            CvMemBlock* block;
            CvMemBlock* dst_top = null;

            if (storage == null) return;

            if (storage->parent != null)
                dst_top = storage->parent->top;

            for (block = storage->bottom; block != null; k++)
            {
                CvMemBlock* temp = block;

                block = block->next;
                if (storage->parent != null)
                {
                    if (dst_top != null)
                    {
                        temp->prev = dst_top;
                        temp->next = dst_top->next;
                        if (temp->next != null)
                            temp->next->prev = temp;
                        dst_top = dst_top->next = temp;
                    }
                    else
                    {
                        dst_top = storage->parent->bottom = storage->parent->top = temp;
                        temp->prev = temp->next = null;
                        storage->free_space = storage->block_size - sizeof(CvMemBlock);
                    }
                }
                else
                {
                    Marshal.FreeHGlobal((IntPtr)temp);
                }
            }

            storage->top = storage->bottom = null;
            storage->free_space = 0;
        }

        public static void cvReleaseMemStorage(CvMemStorage** storage)
        {
            if (storage == null) return;

            CvMemStorage* st = *storage;
            *storage = null;
            if (st != null)
            {
                icvDestroyMemStorage(st);
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

        public static void* Alloc(CvMemStorage* storage, int size)
        {
            Byte* ptr = null;

            if (storage == null) return null;

            if (size > Int32.MaxValue) return null;

            if ((SizeT)storage->free_space < size)
            {
                SizeT max_free_space = AlignLeft(storage->block_size - sizeof(CvMemBlock), 8);
                if (max_free_space < size) return null;

                icvGoNextMemBlock(storage);
            }

            ptr = ((Byte*)(storage)->top + (storage)->block_size - (storage)->free_space);
            storage->free_space = AlignLeft(storage->free_space - (int)size, 8);

            return ptr;
        }

        /* Remember memory storage position: */
        public static void cvSaveMemStoragePos(CvMemStorage* storage, CvMemStoragePos* pos)
        {
            if (storage == null || pos == null) return;

            pos->top = storage->top;
            pos->free_space = storage->free_space;
        }


        /// <summary>
        /// Restore memory storage position
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="pos"></param>
        public static void cvRestoreMemStoragePos(CvMemStorage* storage, CvMemStoragePos* pos)
        {
            if (storage == null || pos == null) return;

            if (pos->free_space > storage->block_size) throw new Exception("bad size");

            storage->top = pos->top;
            storage->free_space = pos->free_space;

            if (storage->top == null)
            {
                storage->top = storage->bottom;
                storage->free_space = (storage->top != null) ? storage->block_size - sizeof(CvMemBlock) : 0;
            }
        }

        /// <summary>
        /// Moves stack pointer to next block. 
        /// If no blocks, allocate new one and link it to the storage.
        /// </summary>
        /// <param name="storage"></param>
        public static void icvGoNextMemBlock(CvMemStorage* storage)
        {
            if (storage == null) return;

            if (storage->top==null || storage->top->next == null)
            {
                CvMemBlock* block;

                if ((storage->parent) == null)
                {
                    block = (CvMemBlock*)Marshal.AllocHGlobal(storage->block_size);
                }
                else
                {
                    CvMemStorage* parent = storage->parent;
                    CvMemStoragePos parent_pos;
                    
                    cvSaveMemStoragePos(parent, &parent_pos);
                    icvGoNextMemBlock(parent);

                    block = parent->top;
                    cvRestoreMemStoragePos(parent, &parent_pos);

                    if (block == parent->top)  /* the single allocated block */
                    {
                        parent->top = parent->bottom = null;
                        parent->free_space = 0;
                    }
                    else
                    {
                        /* cut the block from the parent's list of blocks */
                        parent->top->next = block->next;
                        if (block->next != null)
                            block->next->prev = parent->top;
                    }
                }

                /* link block */
                block->next = null;
                block->prev = storage->top;

                if (storage->top != null)
                    storage->top->next = block;
                else
                    storage->top = storage->bottom = block;
            }

            if (storage->top->next != null)
                storage->top = storage->top->next;
            storage->free_space = storage->block_size - sizeof(CvMemBlock);
        }
    }
}
