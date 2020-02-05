
namespace VTS.Receiver
{
    using Microsoft.Extensions.Configuration;

    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using VTS.Parser;
    using VTS.Parser.Messages;
    using VTS.Receiver.Helpers;

    public static class Program
    {

        static Parser parser;
        static RedisDB redis;
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .AddJsonFile("settings.json", true, true)
                            .AddJsonFile("local.settings.json", true, true)
                            .Build();

            //storageClient = new StorageClient(config);
            //storageClient.InitialiseConnection();
            if (parser == null) parser = new Parser();
            redis = new RedisDB(config["RedisCon"],7);
            var IP_AIS =  config["ip-ais"];
            var Port_AIS = int.Parse(config["port-ais"]);
            var receiver = new NmeaReceiver(IP_AIS, Port_AIS);
            receiver.Items.Buffer(100).Subscribe(OnMessageReceived, OnError);

            while (!receiver.Connected)
            {
                await receiver.InitaliseAsync().ConfigureAwait(false);
                await receiver.RecieveAsync().ConfigureAwait(false);
            }
        }

        private static void OnError(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

        private static void OnMessageReceived(IList<string> messages)
        {
            foreach (var message in messages)
            {
                try
                {
                    AisMessage parsed = null;
                    var idx = message.IndexOf('!');
                    if (idx > -1)
                    {
                        var cuttedMessage = message.Substring(idx);
                        if (cuttedMessage.StartsWith("!AIVDM"))
                            parsed = parser.Parse(cuttedMessage);
                        else continue;
                    }
                    else
                    {
                        parsed = parser.Parse(message);
                    }
                     

                    //var obj = AisMessageJsonConvert.Deserialize(message);

                    Object obj=null;
                    switch (parsed.MessageType)
                    {
                        case AisMessageType.AddressedSafetyRelatedMessage:
                            obj = parsed as AddressedSafetyRelatedMessage;

                            break;
                        case AisMessageType.AidToNavigationReport:
                            obj = parsed as AidToNavigationReportMessage;

                            break;
                        /*
                    case AisMessageType.AssignmentModeCommand:
                        obj = parsed as AssignmentModeCommand;

                        break;*/
                        case AisMessageType.BaseStationReport:
                            obj = parsed as BaseStationReportMessage;

                            break;
                        case AisMessageType.BinaryAcknowledge:
                            obj = parsed as BinaryAcknowledgeMessage;

                            break;
                        case AisMessageType.BinaryAddressedMessage:
                            obj = parsed as BinaryAddressedMessage;

                            break;
                        case AisMessageType.BinaryBroadcastMessage:
                            obj = parsed as BinaryBroadcastMessage;

                            break;
                        /*
                    case AisMessageType.ChannelManagement:
                        obj = parsed as ChannelManagement;

                        break;
                    case AisMessageType.DataLinkManagement:
                        obj = parsed as DataLinkManagement;

                        break;
                    case AisMessageType.DgnssBinaryBroadcastMessage:
                        obj = parsed as DgnssBinaryBroadcastMessage;

                        break;*/
                        case AisMessageType.ExtendedClassBCsPositionReport:
                            obj = parsed as ExtendedClassBCsPositionReportMessage;

                            break;
                        /*
                    case AisMessageType.GroupAssignmentCommand:
                        obj = parsed as GroupAssignmentCommand;

                        break;*/
                        case AisMessageType.Interrogation:
                            obj = parsed as InterrogationMessage;

                            break;
                        /*
                    case AisMessageType.MultipleSlotBinaryMessageWithCommunicationsState:
                        obj = parsed as MultipleSlotBinaryMessageWithCommunicationsState;

                        break;*/
                        case AisMessageType.PositionReportClassA:
                            obj = parsed as PositionReportClassAMessage;

                            break;
                        case AisMessageType.PositionReportClassAAssignedSchedule:
                            obj = parsed as PositionReportClassAAssignedScheduleMessage;

                            break;
                        case AisMessageType.PositionReportClassAResponseToInterrogation:
                            obj = parsed as PositionReportClassAResponseToInterrogationMessage;

                            break;
                        case AisMessageType.PositionReportForLongRangeApplications:
                            obj = parsed as PositionReportForLongRangeApplicationsMessage;

                            break;
                        case AisMessageType.SafetyRelatedAcknowledgement:
                            obj = parsed as SafetyRelatedAcknowledgementMessage;

                            break;
                        /*
                    case AisMessageType.SafetyRelatedBroadcastMessage:
                        obj = parsed as SafetyRelatedBroadcastMessage;

                        break;
                    case AisMessageType.SingleSlotBinaryMessage:
                        obj = parsed as SingleSlotBinaryMessage;

                        break;*/
                        case AisMessageType.StandardClassBCsPositionReport:
                            obj = parsed as StandardClassBCsPositionReportMessage;

                            break;
                        case AisMessageType.StandardSarAircraftPositionReport:
                            obj = parsed as StandardSarAircraftPositionReportMessage;
                            
                            break;
                        case AisMessageType.StaticAndVoyageRelatedData:
                            obj = parsed as StaticAndVoyageRelatedDataMessage;

                            break;
                        case AisMessageType.StaticDataReport:
                            obj = parsed as StaticDataReportMessage;

                            break;
                        case AisMessageType.UtcAndDateInquiry:
                            obj = parsed as UtcAndDateInquiryMessage;

                            break;
                        case AisMessageType.UtcAndDateResponse:
                            obj = parsed as UtcAndDateResponseMessage;

                            break;
                    }
                    if (obj != null)
                    {
                        var msg = obj as AisMessage;
                        Console.WriteLine($"{nameof(parsed.MessageType)} {msg}");
                        var insertObj = new DataAIS() { Id = redis.GetSequence<DataAIS>(), Data = msg, Tipe = nameof(parsed.MessageType) };
                        var result = redis.InsertData<DataAIS>(insertObj);
                        Console.WriteLine($"insert to redis : {result}");

                    }
                }catch(Exception ex)
                {
                    Console.WriteLine($"error parsing : {ex.Message}");
                }
                Console.WriteLine($"{message}");
            }
                       
            //storageClient.AppendMessages(messages);
        }

        
    }
}