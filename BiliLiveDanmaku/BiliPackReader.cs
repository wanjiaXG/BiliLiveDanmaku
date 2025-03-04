﻿using BiliLive.Packs;
using BiliLive.Packs.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace BiliLive
{
    public class BiliPackReader
    {
        public Stream BaseStream { get; private set; }
        public ClientWebSocket BaseWebSocket { get; private set; }

        public BiliPackReader(Stream stream)
        {
            BaseStream = stream;
        }

        public BiliPackReader(ClientWebSocket webSocket)
        {
            BaseWebSocket = webSocket;
            BaseStream = new MemoryStream();
        }

        public Pack[] ReadPacksAsync()
        {
            try
            {
                if (BaseWebSocket != null)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                    WebSocketReceiveResult webSocketReceiveResult = BaseWebSocket.ReceiveAsync(buffer, CancellationToken.None).GetAwaiter().GetResult();
                    BaseStream.Position = 0;
                    BaseStream.Write(buffer.Array, 0, webSocketReceiveResult.Count);
                    BaseStream.Position = 0;
                }

                // Pack length (4)
                byte[] packLengthBuffer = ReadTcpStream(BaseStream, 4);
                int packLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(packLengthBuffer, 0));
                if (packLength < 16)
                {
                    BaseStream.Flush();
                    // TODO : 包长度过短
                    throw new Exception();
                }

                // Header length (2)
                byte[] headerLengthBuffer = ReadTcpStream(BaseStream, 2);
                int headerLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(headerLengthBuffer, 0));
                if (headerLength != 16)
                {
                    BaseStream.Flush();
                    // TODO : 头部长度异常
                    throw new Exception();
                }

                // Data type (2)
                byte[] dataTypeBuffer = ReadTcpStream(BaseStream, 2);
                int dataTypeCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(dataTypeBuffer, 0));
                DataTypes dataType;
                if (Enum.IsDefined(typeof(DataTypes), dataTypeCode))
                {
                    dataType = (DataTypes)Enum.ToObject(typeof(DataTypes), dataTypeCode);
                }
                else
                {
                    dataType = DataTypes.Unknow;
                }


                // Read pack type (4)
                byte[] packTypeBuffer = ReadTcpStream(BaseStream, 4);
                int packTypeCode = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(packTypeBuffer, 0));
                PackTypes packType;
                if (Enum.IsDefined(typeof(PackTypes), packTypeCode))
                {
                    packType = (PackTypes)Enum.ToObject(typeof(PackTypes), packTypeCode);
                }
                else
                {
                    packType = PackTypes.Unknow;
                }

                // Read split (4)
                byte[] splitBuffer = ReadTcpStream(BaseStream, 4);

                // Read payload
                int payloadLength = packLength - headerLength;
                byte[] payloadBuffer = ReadTcpStream(BaseStream, payloadLength);

                // Return
                switch (dataType)
                {
                    case DataTypes.Plain:
                        switch (packType)
                        {
                            case PackTypes.Command:
                                return new CommandPack[] { new CommandPack(payloadBuffer) };
                            default:
                                // TODO : 未知包类型
                                throw new Exception();
                        }
                    case DataTypes.Bin:
                        switch (packType)
                        {
                            case PackTypes.Popularity:
                                return new PopularityPack[] { new PopularityPack(payloadBuffer) };
                            case PackTypes.Heartbeat:
                                return new HeartbeatPack[] { new HeartbeatPack(payloadBuffer) };
                            default:
                                // TODO : 未知包类型
                                throw new Exception();
                        }
                    case DataTypes.Gz:
                        List<Pack> packs = new List<Pack>();
                        using (MemoryStream compressedStream = new MemoryStream(payloadBuffer))
                        {
                            using (GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                            {
                                using (MemoryStream decompressedStream = new MemoryStream())
                                {
                                    gZipStream.CopyTo(decompressedStream);
                                    decompressedStream.Position = 0;

                                    while (decompressedStream.Position != decompressedStream.Length)
                                    {
                                        Pack[] innerPackes = new BiliPackReader(decompressedStream).ReadPacksAsync();
                                        packs.AddRange(innerPackes);
                                    }
                                }
                            }
                        }
                        return packs.ToArray();
                    default:
                        // TODO : 未知数据类型
                        throw new Exception();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

        }

        private byte[] ReadTcpStream(Stream stream, int length)
        {
            
            int position = 0;
            byte[] buffer = new byte[length];
            while (position != length)
            {
                position += stream.Read(buffer, position, buffer.Length - position);
            }
            return buffer;
        }
    }
}
