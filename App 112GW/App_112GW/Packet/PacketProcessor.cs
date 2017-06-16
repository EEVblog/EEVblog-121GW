using System;
using System.Collections.Generic;
using System.Text;

namespace rMultiplatform
{
    public class PacketProcessor
    {
        public delegate void ProcessPacket(byte[] pPacket);
        public  event ProcessPacket mCallback;
        private bool mStartFound;
        private byte mStart;
        private int mLength;
        private int mCount;
        private List<byte> mBuffer;
        public PacketProcessor(byte start, int length)
        { 
            mBuffer = new List<byte>();
            mStart = start;
            mLength = length;
            mCount = 0;
            mStartFound = false;
        }
        public void Recieve(byte[] pBytes)
        {
            foreach(var byt in pBytes)
            {
                //Add byte
                if (mStartFound)
                {
                    mBuffer.Add(byt);
                    mCount++;
                    if (mCount >= mLength)
                    {
                        mCallback?.Invoke(mBuffer.ToArray());
                        mBuffer.Clear();
                        mCount = 0;
                        mStartFound = false;
                    }
                }
                if (mCount == 0)
                {
                    if (byt == mStart)
                    {
                        mStartFound = true;
                        mBuffer.Clear();
                    }
                }
                    

            }
        }
    }
}
