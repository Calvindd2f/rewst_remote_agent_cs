namespace Rewst.RemoteAgent.Models
{
    public class DataDirectory
	{
		public uint VirtualAddress
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<VirtualAddress>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<VirtualAddress>k__BackingField = value;
			}
		}

		public uint Size
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<Size>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<Size>k__BackingField = value;
			}
		}

		public static  Rewst.RemoteAgent.Models.DataDirectory Create(byte[] byteContents, bool isLittleEndian = false)
		{
			if ( System.Linq.Enumerable.Count<byte>(byteContents) != 8)
			{
				return null;
			}
			if (!isLittleEndian)
			{
				throw new  System.Exception("Unsupported operation");
			}
			return new  Rewst.RemoteAgent.Models.DataDirectory
			{
				VirtualAddress =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0), 4))),
				Size =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 4), 4)))
			};
		}

		public DataDirectory()
		{
		}

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <VirtualAddress>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <Size>k__BackingField;
	}
}


