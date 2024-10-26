using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Rewst.RemoteAgent.Calvindd2f.Helpers;

namespace Rewst.RemoteAgent.Calvindd2f.Models
{
	internal class OptionalHeader
	{
		public ushort Magic
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<Magic>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<Magic>k__BackingField = value;
			}
		}

		public byte MajorLinkerVersion
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<MajorLinkerVersion>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<MajorLinkerVersion>k__BackingField = value;
			}
		}

		public byte MinorLinkerVersion
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<MinorLinkerVersion>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<MinorLinkerVersion>k__BackingField = value;
			}
		}

		public uint SizeOfCode
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<SizeOfCode>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<SizeOfCode>k__BackingField = value;
			}
		}

		public uint SizeOfInitializedData
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<SizeOfInitializedData>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<SizeOfInitializedData>k__BackingField = value;
			}
		}

		public uint SizeOfUninitializedData
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<SizeOfUninitializedData>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<SizeOfUninitializedData>k__BackingField = value;
			}
		}

		public uint AddressOfEntryPoint
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<AddressOfEntryPoint>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<AddressOfEntryPoint>k__BackingField = value;
			}
		}

		public uint BaseOfCode
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<BaseOfCode>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<BaseOfCode>k__BackingField = value;
			}
		}

		public static  Rewst.RemoteAgent.Calvindd2f.Models.OptionalHeader Create(byte[] byteContents, bool isLittleEndian = false)
		{
			if ( System.Linq.Enumerable.Count<byte>(byteContents) < 0x18)
			{
				return null;
			}
			if (!isLittleEndian)
			{
				throw new  System.Exception("Unsupported operation");
			}
			return new  Rewst.RemoteAgent.Calvindd2f.Models.OptionalHeader
			{
				Magic =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0), 2))),
				MajorLinkerVersion = byteContents[2],
				MinorLinkerVersion = byteContents[3],
				SizeOfCode =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 4), 4))),
				SizeOfInitializedData =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 8), 4))),
				SizeOfUninitializedData =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0xC), 4))),
				AddressOfEntryPoint =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0x10), 4))),
				BaseOfCode =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0x14), 4)))
			};
		}

		public OptionalHeader()
		{
		}

		public const int Size = 0x18;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private ushort <Magic>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private byte <MajorLinkerVersion>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private byte <MinorLinkerVersion>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <SizeOfCode>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <SizeOfInitializedData>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <SizeOfUninitializedData>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <AddressOfEntryPoint>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <BaseOfCode>k__BackingField;
	}
}


