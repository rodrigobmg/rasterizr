﻿namespace Rasterizr.Resources
{
	public abstract class ResourceView : DeviceChild
	{
		private readonly Resource _resource;

		public Resource Resource
		{
			get { return _resource; }
		}

		protected ResourceView(Device device, Resource resource)
			: base(device)
		{
			_resource = resource;
		}
	}
}