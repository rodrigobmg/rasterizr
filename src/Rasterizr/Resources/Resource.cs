﻿using Rasterizr.Util;

namespace Rasterizr.Resources
{
	public abstract class Resource
	{
		private readonly Device _device;
		private readonly byte[] _data;

		protected Resource(Device device, int sizeInBytes)
		{
			_device = device;
			_data = new byte[sizeInBytes];
		}

		public void GetData<T>(int dataOffset, T[] data, int startIndex, int elementCount)
			where T : struct
		{
			Utilities.FromByteArray(data, dataOffset, _data, startIndex, elementCount);
		}

		public void GetData<T>(T[] data, int startIndex, int elementCount)
			where T : struct
		{
			GetData(0, data, startIndex, elementCount);
		}

		public void GetData<T>(T[] data)
			where T : struct
		{
			GetData(data, 0, _data.Length / Utilities.SizeOf<T>());
		}

		public void SetData<T>(T[] data)
			where T : struct
		{
			Utilities.ToByteArray(data, _data);
		}
	}
}