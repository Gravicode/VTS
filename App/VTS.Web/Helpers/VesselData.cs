using GoogleMapsComponents.Maps;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VTS.Parser;
using VTS.Parser.Messages;
using VTS.Web.Data;

namespace VTS.Web.Helpers
{
    public class VesselData
    {
        bool IsProcessing = false;
        long LastIndex = 0;
        public Dictionary<uint,List<AisMessage>> ShipDatas { get; set; }
        public Dictionary<uint, List<AisMessage>> ShipPositions { get; set; }
        RedisDB redis; 

 
        public VesselData(RedisDB redis)
        {
            if (ShipDatas == null)
                ShipDatas = new Dictionary<uint, List<AisMessage>>();
            if (ShipPositions == null)
                ShipPositions = new Dictionary<uint, List<AisMessage>>();
            this.redis = redis;
        }

        public void AddShipData(StaticAndVoyageRelatedDataMessage item)
        {
            if (ShipDatas.ContainsKey(item.Mmsi))
            {
                ShipDatas[item.Mmsi].Add(item);
            }
            else
            {
                ShipDatas.Add(item.Mmsi, new List<AisMessage>() { item });
            }
        }

        public void AddShipPosition(AisMessage item)
        {
            if(item is PositionReportClassAMessage || item is ExtendedClassBCsPositionReportMessage)
            {
                if (ShipPositions.ContainsKey(item.Mmsi))
                {
                    ShipPositions[item.Mmsi].Add(item);
                }
                else
                {
                    ShipPositions.Add(item.Mmsi, new List<AisMessage>() { item });
                }
            }
          
        }

        public bool QueryDataFromDB()
        {
            bool IsExist = false;
            if (IsProcessing) return IsExist;
            IsProcessing = true;
            try
            {
            var allData = redis.GetAllData<DataAIS>();
            if(allData!=null && LastIndex > 0)
            {
                allData = allData.Where(x => x.Id > LastIndex).ToList();
            }
            if (allData != null && allData.Count > 0)
            {
                foreach (var item in allData)
                {
                    switch (item.Data)
                    {
                        case PositionReportClassAMessage obj:
                            AddShipPosition(obj);
                            IsExist = true;
                            break;
                        case ExtendedClassBCsPositionReportMessage obj2:
                            AddShipPosition(obj2);
                            IsExist = true;
                            break;
                        case StaticAndVoyageRelatedDataMessage obj3:
                            AddShipData(obj3);
                            IsExist = true;
                            break;
                    }
                    LastIndex = item.Id;
                }
                }
            }
            finally
            {
                IsProcessing = false;
            }
            return IsExist;
        }

        public List<VesselInfo> GetAllShipInArea(List<LatLngLiteral> Area)
        {
            var list = new List<VesselInfo>();
            try
            {



                foreach (var item in ShipPositions.Values)
                {
                    PointXY loc;
                    VesselInfo data = null;
                    if (item[item.Count - 1] is PositionReportClassAMessage)
                    {

                        var obj = item[item.Count - 1] as PositionReportClassAMessage;
                        if (!ShipDatas.ContainsKey(obj.Mmsi)) continue;
                        var shipInfo = ShipDatas[obj.Mmsi].Last() as StaticAndVoyageRelatedDataMessage;

                        loc = new PointXY(obj.Latitude, obj.Longitude);
                        data = new VesselInfo() { Mmsi = obj.Mmsi, Lat = obj.Latitude, Course = obj.CourseOverGround, Dest = shipInfo.Destination, ETA = $"{shipInfo.EtaDay}/{shipInfo.EtaMonth} {shipInfo.EtaHour}:{shipInfo.EtaMinute}", Lng = obj.Longitude, ShipName = shipInfo.ShipName, Speed = obj.SpeedOverGround, Status = obj.NavigationStatus.ToString(), Direction = obj.TrueHeading };
                    }
                    else if (item[item.Count - 1] is ExtendedClassBCsPositionReportMessage)
                    {
                        var obj = item[item.Count - 1] as ExtendedClassBCsPositionReportMessage;
                        if (!ShipDatas.ContainsKey(obj.Mmsi)) continue;
                        var shipInfo = ShipDatas[obj.Mmsi].Last() as StaticAndVoyageRelatedDataMessage;

                        loc = new PointXY(obj.Latitude, obj.Longitude);
                        data = new VesselInfo() { Mmsi = obj.Mmsi, Lat = obj.Latitude, Course = obj.CourseOverGround, Dest = shipInfo.Destination, ETA = $"{shipInfo.EtaDay}/{shipInfo.EtaMonth} {shipInfo.EtaHour}:{shipInfo.EtaMinute}", Lng = obj.Longitude, ShipName = shipInfo.ShipName, Speed = obj.SpeedOverGround, Status = "", Direction = obj.TrueHeading };

                    }
                    if (data != null)
                        if (Area == null)
                        {
                            list.Add(data);
                        }
                        else if(GeoFence.PointInPolygon(new LatLngLiteral() { Lat=data.Lat, Lng=data.Lng }, Area))
                        {
                            list.Add(data);
                        }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("get ships error : " + ex.Message);
            }
            return list;
        }

    }
}
