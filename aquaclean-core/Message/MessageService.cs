using Geberit.AquaClean.Core.Common;
using System;
using System.Diagnostics;

namespace Geberit.AquaClean.Core.Message 
{
    class MessageService
    {
        internal MessageContext ParseMessage(byte[] data)
        {
            var message = Message.CreateFromStream(data);

            Debug.WriteLine(String.Format("Parsing data to message with ID={0} Data={1}", message.ID, data.ToByteString()));

            switch (message.ID)
            {
                case 5:
                    var crcMessage = message as CrcMessage;
                    if (!crcMessage.IsValid)
                    {
                        Debug.WriteLine("Message invalid");
                    }
                    var context = crcMessage.Body[2];
                    var procedure = crcMessage.Body[3];
                    var argByteLength = crcMessage.Body[4];

                    // Copy argument bytes into a new array
                    byte[] argBytes = new byte[argByteLength];
                    Array.Copy(crcMessage.Body, 5, argBytes, 0, argByteLength);

                    // Build return arguments from bytes
                    return ParseMessage(context, procedure, argBytes);
                default:
                    Debug.WriteLine(String.Format("Unknown message ID={0}", message.ID));
                    break;
            }

            return null;
        }


        private MessageContext ParseMessage(byte context, byte procedure, byte[] data)
        {
            Debug.WriteLine(String.Format("Parsing message with Context={0:X2}, Procedure={1:X2}, Data={2}", context, procedure, data.ToByteString()));

            return new MessageContext()
            {
                Context = context,
                Procedure = procedure,
                ResultBytes = data,
            };
        }

        public Message BuildMessage(byte[] data)
        {
            return BuildMessageSegmentOfType(4, data, 0, 1);
        }

        public Message BuildMessageSegmentOfType(byte messageId, byte[] data, byte isZero, byte isOne)
        {
            short messageSegment = (short)(isZero - 1 + ((isOne - 1) * 16));

            switch (messageId)
            {
                case 4:
                    if (CrcMessage.SizeOfHeader() + data.Length > 256)
                    {
                        return null; //false;
                    }
                    return CrcMessage.Create(messageId, messageSegment, data);

                default:
                    return null; // false
            }

        }


    }

}



