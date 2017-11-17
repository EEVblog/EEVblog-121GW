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
		private List<byte> mBuffer;
		public PacketProcessor(byte start, int length)
		{ 
			mBuffer	 = new List<byte>();
			mStart	  = start;
			mLength	 = length;
			mStartFound = false;
		}
		public void Reset()
		{
			mBuffer.Clear();
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
					if (mBuffer.Count >= mLength)
					{
						mCallback?.Invoke(mBuffer.ToArray());
						mBuffer.Clear();
						mStartFound = false;
					}
				}
				if (mBuffer.Count == 0)
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
