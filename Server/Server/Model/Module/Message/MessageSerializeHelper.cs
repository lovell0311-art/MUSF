using System;
using System.IO;
using MongoDB.Bson;

namespace ETModel
{
    public static class MessageSerializeHelper
    {
        public static object DeserializeFrom(ushort opcode, Type type, MemoryStream memoryStream)
        {
            if (OpcodeHelper.IsClientHotfixMessage(opcode))
            {
                return ProtobufHelper.FromStream(type, memoryStream);
            }

            return MongoHelper.FromStream(type, memoryStream);
        }

        public static void SerializeTo(ushort opcode, object obj, MemoryStream memoryStream)
        {
            try
            {
                if (OpcodeHelper.IsClientHotfixMessage(opcode))
                {
                    ProtobufHelper.ToStream(obj, memoryStream);
                    return;
                }

                MongoHelper.ToBson(obj, memoryStream);
            }
            catch (Exception e)
            {
                throw new Exception($"SerializeTo error: {opcode}", e);
            }

        }

    }
}