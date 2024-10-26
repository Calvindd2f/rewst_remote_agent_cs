using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Rewst.RemoteAgent.Calvindd2f.Helpers
{
    public class ByteHelper
    {
        public static byte[] Slice(byte[] arr, int x, int y)
        {
            return System.Linq.Enumerable.ToArray<byte>(
                System.Linq.Enumerable.Take<byte>(System.Linq.Enumerable.Skip<byte>(arr, x), y - x)
            );
        }

        public static uint CalculateLittleEndianToUInt32(byte[] byteContents)
        {
            return System.BitConverter.ToUInt32(byteContents, 0);
        }

        public static ushort CalculateLittleEndianToUInt16(byte[] byteContents)
        {
            return System.BitConverter.ToUInt16(byteContents, 0);
        }

        public static byte ConvertCharToByte(char c)
        {
            string text = c.ToString();
            return System.Text.Encoding.ASCII.GetBytes(text)[0];
        }

        public static string ConvertByteToString(byte[] byteContents)
        {
            if (byteContents == null || byteContents.Length == 0)
            {
                return "";
            }
            string result;
            using (
                System.IO.StreamReader streamReader = new System.IO.StreamReader(
                    new System.IO.MemoryStream(byteContents),
                    System.Text.Encoding.UTF8
                )
            )
            {
                result = string.Join<char>("", streamReader.ReadToEnd().ToCharArray());
            }
            return result;
        }

        public ByteHelper() { }

        public const ushort Pe32Magic = 0x10B;

        public const ushort Pe32PlusMagic = 0x20B;

        public const int OffsetOfPEHeaderOffset = 0x3C;

        public const int CertificateTableIndex = 4;

        public const ushort CoffCharacteristicExecutableImage = 2;

        public const int CoffCharacteristicDLL = 0x2000;

        public const uint AttributeCertificateRevision = 0x200U;

        public const uint AttributeCertificateTypePKCS7SignedData = 2U;
    }
}
