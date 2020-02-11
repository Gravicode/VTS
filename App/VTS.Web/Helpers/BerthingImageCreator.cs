using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VTS.Web.Data;

namespace VTS.Web.Helpers
{
    public class VesselDataExt:VTS.Shared.VesselData
    {
        public Point X1 { get; set; }
        public Point X2 { get; set; }
        public int RowNum { get; set; } = 1;
        public bool IsAssigned { get; set; } = false;
    }
    public class BerthingImageCreator
    {
        
        public static (string FileName, bool Result,List<VesselDataExt> Vessels) CreateImage(List<VTS.Shared.VesselData> Data, int Width = 1200, int Height = 612)
        {
            List<VesselDataExt> VesselInfos = new List<VesselDataExt>();
            var Zones = new List<ZoneInfo>() { new ZoneInfo() { No=0, Title="Plan", ZoneName="Plan"  }, new ZoneInfo() { No=1, Title = "Anchor", ZoneName = "Anchor" },
            new ZoneInfo() { No=2, Title="Realisasi", ZoneName="Berthing"  }};
           
            const int PaddingBottom = 10;
            const int PortWidth = 1000; //meters
            const int ScaleStep = 50;
            var xScale = Width / (float)PortWidth;
            var pen = new Pen(Brushes.Black, 2f);
            var fontSmall = new Font("Arial", 6f);
            var fontTitle = new Font("Arial", 12f);
            var fontNormal = new Font("Arial", 8f);
            System.Drawing.StringFormat drawVertical = new System.Drawing.StringFormat();
            drawVertical.FormatFlags = StringFormatFlags.DirectionVertical;
            StringFormat drawCenter = new StringFormat();
            drawCenter.Alignment = StringAlignment.Center;
            drawCenter.LineAlignment = StringAlignment.Center;
            bool result = false;
            string ImagePath = Path.GetTempFileName() + ".jpg";
            try
            {
                var resized = new Bitmap(Width, Height);
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.Clear(Color.White);
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //graphics.CompositingMode = CompositingMode.SourceCopy;
                    //graphics.DrawImage(resized, 0, 0, Width, Height);
                   
                    //draw bottom line
                    graphics.DrawLine(pen, 0, Height - PaddingBottom, Width, Height - PaddingBottom);
                    //draw scale
                    for (int x = 0; x <= PortWidth; x += ScaleStep)
                    {
                        var ax = x * xScale;
                        graphics.DrawLine(pen, ax, Height, ax, Height - PaddingBottom);
                        if(x< PortWidth)
                            graphics.DrawString($"{x}m", fontSmall, Brushes.Black, new PointF(ax + 5, Height - (PaddingBottom - 2)));
                    }
                    Height = Height - (PaddingBottom+2);
                    //divide zone height and print line and title zone
                    int counter = 0;
                    foreach(var zone in Zones)
                    {
                        zone.Height = Height / Zones.Count;
                        var ay = zone.Height * counter;
                        graphics.DrawLine(pen, 0, ay, Width, ay);
                        graphics.DrawString($"{zone.Title}", fontTitle, Brushes.Black, new PointF(5, ay+5));
                        //calculate number of row
                        var temp = (from x in Data
                                    where x.Info.Status == zone.ZoneName
                                    orderby x.Activity.EstFromMeter, x.Schedule.EstTimeArrival
                                    select new VesselDataExt() { Id = x.Id, Activity = x.Activity, Info= x.Info, IsAssigned=false, Mmsi=x.Mmsi, RowNum=1, Schedule=x.Schedule } );
                        var shipsInZone = new List<VesselDataExt>();
                        if (temp != null)
                        {
                            shipsInZone = temp.ToList();
                        }
                        //assign row number for each ship
                        if(shipsInZone!=null && shipsInZone.Count > 0)
                        {
                            for(int i=0;i<shipsInZone.Count;i++)
                            {
                                var ship = shipsInZone[i];
                                if (!ship.IsAssigned)
                                {
                                    ship.IsAssigned = true;
                                    var SameRow = shipsInZone.Where(x => !x.IsAssigned && ship.Mmsi != x.Mmsi && ((x.Activity.EstFromMeter < ship.Activity.EstFromMeter && x.Activity.EstToMeter > ship.Activity.EstFromMeter) ||
                                    (x.Activity.EstFromMeter < ship.Activity.EstToMeter && x.Activity.EstToMeter > ship.Activity.EstToMeter))).OrderBy(x => x.Activity.EstFromMeter).OrderBy(x => x.Schedule.EstTimeArrival).ToList();
                                    if (SameRow != null && SameRow.Count > 0)
                                    {
                                        zone.RowCount = SameRow.Count + 1;
                                        var rowNum = 2;
                                        foreach (var xx in SameRow)
                                        {
                                            xx.IsAssigned = true;
                                            xx.RowNum = rowNum;
                                            rowNum++;
                                        }
                                    }
                                }
                            }
                        }
                        //draw ship
                       
                        foreach(var ship in shipsInZone)
                        {
                            var heightShip = zone.Height / zone.RowCount;
                            var Dy = (zone.Height * (zone.No + 1)) - (heightShip*ship.RowNum);
                            DrawShip(ship, heightShip, Dy);
                           
                        }
                        void DrawShip(VesselDataExt ves, float ShipHeight,float TopY)
                        {
                            //draw bottom box
                            var ay = (0.8f * ShipHeight) + TopY;
                            var heightTemp = (0.2f * ShipHeight) ;
                            graphics.FillRectangle(Brushes.Silver, new RectangleF(ves.Activity.EstFromMeter * xScale,ay,(ves.Activity.EstToMeter * xScale - ves.Activity.EstFromMeter * xScale), heightTemp));
                            //draw ship name
                            var rectTitle = new RectangleF(ves.Activity.EstFromMeter * xScale + 15, ay + 5,((ves.Activity.EstToMeter * xScale - 15) - (ves.Activity.EstFromMeter * xScale + 15)), heightTemp);
                            graphics.DrawString($"{ves.Info.VesselName}", fontNormal, Brushes.Black, rectTitle,drawCenter);
                            //draw from and to
                            graphics.DrawString($"{ves.Activity.EstFromMeter}m", fontNormal, Brushes.Black, new PointF(ves.Activity.EstFromMeter * xScale, ay + 5),drawVertical);
                            
                            graphics.DrawString($"{ves.Activity.EstToMeter}m", fontNormal, Brushes.Black, new PointF((ves.Activity.EstToMeter * xScale) - 15, ay + 5),drawVertical);

                            ves.X1 = new Point((int)(ves.Activity.EstFromMeter * xScale),(int)TopY);
                            ves.X2 = new Point((int)(ves.Activity.EstToMeter * xScale),(int)(TopY+ShipHeight));
                            //draw desc
                            ay = (0.4f * ShipHeight) + TopY;
                            heightTemp = (0.4f * ShipHeight);
                            var desc = string.Empty;
                            switch (zone.ZoneName)
                            {
                                case "Plan":
                                    desc = $"{ves.Info.Status}:{ves.Schedule.EstTimeArrival.ToString("dd/MM/yy HH:mm")} - B:{ves.Activity.EstDischarge} {ves.Info.FromPort} M:{ves.Activity.EstLoad} {ves.Info.ToPort}";
                                    break;
                                case "Anchor":
                                    desc = $"{ves.Info.Status}:{ves.Schedule.RealTimeAnchor.ToString("dd/MM/yy HH:mm")} {ves.Activity.EstEquipmentName} - B:{ves.Activity.EstDischarge} {ves.Info.FromPort} M:{ves.Activity.EstLoad} {ves.Info.ToPort}";
                                    break;
                                case "Berthing":
                                    desc = $"{ves.Info.Status}:{ves.Schedule.EstTimeBerthing.ToString("dd/MM/yy HH:mm")} {ves.Activity.EstEquipmentName} - B:{ves.Activity.EstDischarge} {ves.Info.FromPort} M:{ves.Activity.EstLoad} {ves.Info.ToPort}";
                                    break;
                            }
                            rectTitle = new RectangleF(ves.Activity.EstFromMeter * xScale , ay + 5, ((ves.Activity.EstToMeter * xScale) - (ves.Activity.EstFromMeter * xScale )), heightTemp);
                            graphics.DrawString($"{desc}", fontNormal, Brushes.Black, rectTitle, drawCenter);
                        }
                        VesselInfos.AddRange(shipsInZone);
                        counter++;
                    }
                    

                    result = true;

                }
                resized.Save(ImagePath, ImageFormat.Png);
                Console.WriteLine($"Saving image -> {ImagePath}");
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message+"_"+ex.StackTrace);
            }
            return (ImagePath, result, VesselInfos);
           
        }
    }
}
