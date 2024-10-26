using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Rewst.RemoteAgent.Calvindd2f.Helpers;

namespace Rewst.RemoteAgent.Calvindd2f.Models
{
	public class FileHeader
	{
		public ushort Machine
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<Machine>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<Machine>k__BackingField = value;
			}
		}

		public ushort NumberOfSections
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<NumberOfSections>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<NumberOfSections>k__BackingField = value;
			}
		}

		public uint TimeDateStamp
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<TimeDateStamp>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<TimeDateStamp>k__BackingField = value;
			}
		}

		public uint PointerForSymbolTable
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<PointerForSymbolTable>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<PointerForSymbolTable>k__BackingField = value;
			}
		}

		public uint NumberOfSymbols
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<NumberOfSymbols>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<NumberOfSymbols>k__BackingField = value;
			}
		}

		public ushort SizeOfOptionalHeader
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<SizeOfOptionalHeader>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<SizeOfOptionalHeader>k__BackingField = value;
			}
		}

		public ushort Characteristics
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<Characteristics>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<Characteristics>k__BackingField = value;
			}
		}

		public static  Rewst.RemoteAgent.Calvindd2f.Models.FileHeader Create(byte[] byteContents, bool isLittleEndian = false)
		{
			if ( System.Linq.Enumerable.Count<byte>(byteContents) != 0x14)
			{
				return null;
			}
			if (!isLittleEndian)
			{
				throw new  System.Exception("Unsupported operation");
			}
			return new  Rewst.RemoteAgent.Calvindd2f.Models.FileHeader
			{
				Machine =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0), 2))),
				NumberOfSections =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 2), 2))),
				TimeDateStamp =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 4), 4))),
				PointerForSymbolTable =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 8), 4))),
				NumberOfSymbols =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0xC), 4))),
				SizeOfOptionalHeader =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0x10), 2))),
				Characteristics =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0x12), 2)))
			};
		}

		public FileHeader()
		{
		}

		public const int Size = 0x14;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private ushort <Machine>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private ushort <NumberOfSections>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <TimeDateStamp>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <PointerForSymbolTable>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private uint <NumberOfSymbols>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private ushort <SizeOfOptionalHeader>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private ushort <Characteristics>k__BackingField;
	}
}


