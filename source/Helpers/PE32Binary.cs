using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Rewst.RemoteAgent.Calvindd2f.Models;

namespace Rewst.RemoteAgent.Calvindd2f.Helpers
{
	public class PE32Binary
	{
		public byte[] Contents
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<Contents>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<Contents>k__BackingField = value;
			}
		}

		public int AttrCertOffset
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<AttrCertOffset>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<AttrCertOffset>k__BackingField = value;
			}
		}

		public int CertSizeOffset
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<CertSizeOffset>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<CertSizeOffset>k__BackingField = value;
			}
		}

		public byte[] Asn1Bytes
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<Asn1Bytes>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<Asn1Bytes>k__BackingField = value;
			}
		}

		public byte[] AppendedTag
		{
			[ System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return this.<AppendedTag>k__BackingField;
			}
			[ System.Runtime.CompilerServices.CompilerGenerated]
			set
			{
				this.<AppendedTag>k__BackingField = value;
			}
		}

		public static  Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary Create(byte[] byteContents)
		{
			 System.Tuple<int, int, int> attributeCertificates =  Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary.GetAttributeCertificates(byteContents);
			int item = attributeCertificates.Item1;
			int item2 = attributeCertificates.Item2;
			int item3 = attributeCertificates.Item3;
			 System.Tuple<byte[], byte[]> tuple =  Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary.ProcessAttributeCertificates( Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(byteContents, item, item + item2));
			byte[] item4 = tuple.Item1;
			byte[] item5 = tuple.Item2;
			return new  Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary
			{
				Contents = byteContents,
				AttrCertOffset = item,
				CertSizeOffset = item3,
				AppendedTag = item5,
				Asn1Bytes = item4
			};
		}

		public string GetAppendedTag()
		{
			if (this.AppendedTag.Length == 0)
			{
				return "";
			}
			return  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.ConvertByteToString(this.AppendedTag);
		}

		private static  System.Tuple<int, int, int> GetAttributeCertificates(byte[] byteContents)
		{
			if ( System.Linq.Enumerable.Count<byte>(byteContents) < 0x40)
			{
				throw new  System.Exception("GetAttributeCertificates: Binary truncated.");
			}
			uint num =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Skip<byte>(byteContents, 0x3C)));
			if (num < 0U || num + 4U < num)
			{
				throw new  System.Exception("GetAttributeCertificates: Overflow finding PE signature.");
			}
			if ((long) System.Linq.Enumerable.Count<byte>(byteContents) < (long)((ulong)(num + 4U)))
			{
				throw new  System.Exception("GetAttributeCertificates: Binary truncated.");
			}
			byte[] array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(byteContents, (int)num,  System.Linq.Enumerable.Count<byte>(byteContents));
			byte[] array2 = new byte[4];
			array2[0] =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.ConvertCharToByte('P');
			array2[1] =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.ConvertCharToByte('E');
			byte[] array3 = array2;
			byte[] array4 =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 0, 4);
			for (int i = 0; i < array4.Length; i++)
			{
				if (!array4[i].Equals(array3[i]))
				{
					throw new  System.Exception("GetAttributeCertificates: PE header not found at expected offset.");
				}
			}
			array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 4,  System.Linq.Enumerable.Count<byte>(array));
			 Rewst.RemoteAgent.Calvindd2f.Models.FileHeader fileHeader =  Rewst.RemoteAgent.Calvindd2f.Models.FileHeader.Create( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>(array, 0x14)), true);
			array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 0x14,  System.Linq.Enumerable.Count<byte>(array));
			if (((int)(fileHeader.Characteristics & 2)).Equals(0))
			{
				throw new  System.Exception("GetAttributeCertificates: File is not an executable image.");
			}
			if (((int)(fileHeader.Characteristics & 0x2000)).CompareTo(0) != 0)
			{
				throw new  System.Exception("GetAttributeCertificates: File is a dll.");
			}
			 Rewst.RemoteAgent.Calvindd2f.Models.OptionalHeader optionalHeader =  Rewst.RemoteAgent.Calvindd2f.Models.OptionalHeader.Create( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>(array, (int)fileHeader.SizeOfOptionalHeader)), true);
			array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 0x18,  System.Linq.Enumerable.Count<byte>(array));
			int num2;
			if (optionalHeader.Magic == 0x20B)
			{
				num2 = 8;
			}
			else
			{
				if (optionalHeader.Magic != 0x10B)
				{
					throw new  System.Exception("GetAttributeCertificates: Unknown magic in optional header: " + optionalHeader.Magic.ToString());
				}
				num2 = 4;
				array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 4,  System.Linq.Enumerable.Count<byte>(array));
			}
			int num3 = num2 + 0x28 + num2 * 4 + 4;
			array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, num3,  System.Linq.Enumerable.Count<byte>(array));
			uint num4 =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>(array, 4)));
			array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 4,  System.Linq.Enumerable.Count<byte>(array));
			if (num4 > 0x1000U)
			{
				throw new  System.Exception("GetAttributeCertificates: Invalid number of directory entries:" + num4.ToString());
			}
			num3 = 0;
			 System.Collections.Generic.List< Rewst.RemoteAgent.Calvindd2f.Models.DataDirectory> list = new  System.Collections.Generic.List< Rewst.RemoteAgent.Calvindd2f.Models.DataDirectory>();
			int num5 = 0;
			while ((long)num5 < (long)((ulong)num4))
			{
				 Rewst.RemoteAgent.Calvindd2f.Models.DataDirectory dataDirectory =  Rewst.RemoteAgent.Calvindd2f.Models.DataDirectory.Create( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>( System.Linq.Enumerable.Skip<byte>(array, num3), 8)), true);
				list.Add(dataDirectory);
				num3 += 8;
				num5++;
			}
			array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, num3,  System.Linq.Enumerable.Count<byte>(array));
			if (num4 <= 4U)
			{
				throw new  System.Exception("GetAttributeCertificates: File does not have enough data directory entries for a certificate");
			}
			 Rewst.RemoteAgent.Calvindd2f.Models.DataDirectory dataDirectory2 = list[4];
			if (dataDirectory2.VirtualAddress == 0U)
			{
				throw new  System.Exception("GetAttributeCertificates: File does not have certificate data.");
			}
			uint num6 = dataDirectory2.VirtualAddress + dataDirectory2.Size;
			if (num6 < dataDirectory2.VirtualAddress)
			{
				throw new  System.Exception("GetAttributeCertificates: Overflow while calculating end of certificate entry");
			}
			if ((ulong)num6 != (ulong)((long) System.Linq.Enumerable.Count<byte>(byteContents)))
			{
				throw new  System.Exception(string.Format("GetAttributeCertificates: Certificate entry is not at end of file: {0} vs {1}.", num6, byteContents.Length));
			}
			int virtualAddress = (int)dataDirectory2.VirtualAddress;
			int size = (int)dataDirectory2.Size;
			uint num7 = num + 4U + 0x14U + (uint)fileHeader.SizeOfOptionalHeader - 8U * (num4 - 4U) + 4U;
			if ( Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(byteContents, (int)num7,  System.Linq.Enumerable.Count<byte>(byteContents))) != dataDirectory2.Size)
			{
				throw new  System.Exception("GetAttributeCertificates: Internal error when calculating certificate data size offset.");
			}
			return new  System.Tuple<int, int, int>(virtualAddress, size, (int)num7);
		}

		public static string GetTenantName()
		{
			string result;
			try
			{
				result =  Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary.Create( System.IO.File.ReadAllBytes( System.Windows.Forms.Application.ExecutablePath)).GetAppendedTag();
			}
			catch ( System.Exception e)
			{
				 Rewst.RemoteAgent.Calvindd2f.LogUtil.LogError("Get tenant name failed. Setting it to empty", "Exception when get TenantName. " +  Rewst.RemoteAgent.Calvindd2f.ExceptionLoggingUtil.GetExceptionInformation(e));
				result = string.Empty;
			}
			return result;
		}

		private static int GetLengthAsn1(byte[] asn1)
		{
			int num;
			if (((int)(asn1[1] & 0x80)).Equals(0))
			{
				num = (int)(asn1[1] + 2);
			}
			else
			{
				int num2 = (int)(asn1[1] & 0x7F);
				if (num2 == 0 || num2 > 2)
				{
					throw new  System.Exception("GetLengthAsn1: Bad number of bytes in ASN.1 length: " + num2.ToString());
				}
				if (asn1.Length < num2 + 2)
				{
					throw new  System.Exception("GetLengthAsn1: ASN.1 structure truncated.");
				}
				num = (int)asn1[2];
				if (num2 == 2)
				{
					num <<= 8;
					num |= (int)asn1[3];
				}
				num += 2 + num2;
			}
			return num;
		}

		private static  System.Tuple<byte[], byte[]> ProcessAttributeCertificates(byte[] attributeCertificates)
		{
			if ( System.Linq.Enumerable.Count<byte>(attributeCertificates) < 8)
			{
				throw new  System.Exception("ProcessAttributeCertificates: Attribute certificate truncated.");
			}
			ulong num = (ulong) Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt32( System.Linq.Enumerable.ToArray<byte>( System.Linq.Enumerable.Take<byte>(attributeCertificates, 4)));
			ushort num2 =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(attributeCertificates, 4, 6));
			ushort num3 =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.CalculateLittleEndianToUInt16( Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(attributeCertificates, 6, 8));
			if (num != (ulong)((long)attributeCertificates.Length))
			{
				throw new  System.Exception("ProcessAttributeCertificates: Multiple attribute certificates found.");
			}
			if (num2 != 0x200)
			{
				throw new  System.Exception("ProcessAttributeCertificates: Unknown attribute certificate revision: " + num2.ToString());
			}
			if (num3 != 2)
			{
				throw new  System.Exception("ProcessAttributeCertificates: Unknown attribute certificate type: " + num3.ToString());
			}
			byte[] array =  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(attributeCertificates, 8, attributeCertificates.Length);
			if (array.Length < 2)
			{
				throw new  System.Exception("ProcessAttributeCertificates: ASN.1 structure truncated.");
			}
			int lengthAsn =  Rewst.RemoteAgent.Calvindd2f.Helpers.PE32Binary.GetLengthAsn1(array);
			return new  System.Tuple<byte[], byte[]>( Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, 0, lengthAsn),  Rewst.RemoteAgent.Calvindd2f.Helpers.ByteHelper.Slice(array, lengthAsn, array.Length));
		}

		public PE32Binary()
		{
		}

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private byte[] <Contents>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private int <AttrCertOffset>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private int <CertSizeOffset>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private byte[] <Asn1Bytes>k__BackingField;

		[ System.Runtime.CompilerServices.CompilerGenerated]
		private byte[] <AppendedTag>k__BackingField;
	}
}


