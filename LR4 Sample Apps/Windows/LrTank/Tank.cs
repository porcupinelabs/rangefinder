using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace LxDemo
{
    class Tank
    {
        private double overallHeight;
        private double eavesHeight;
        private double hopperHeight;
        private double hopperAngle;
        private double drainHeight;
        private double binDiameter;
        private double peakOpenning;
        private double roofAngle;
        private double laserOffset;
        private double materialDensity;
        private bool fillingMode;
        private double tankLevelAsDist;
        private Polygon materialPoly;
        private double xOffset, yOffset;
        
        public Tank(string IniSectionName, Canvas c, double x, double y)
        {
            IniFile ini = new IniFile((AppDomain.CurrentDomain.BaseDirectory) + "lrtank.ini");
            this.overallHeight   = ini.IniReadDouble(IniSectionName, "OverallHeight", 15.773);
            this.eavesHeight     = ini.IniReadDouble(IniSectionName, "EavesHeight", 14.046);
            this.hopperHeight    = ini.IniReadDouble(IniSectionName, "HopperHeight", 3.937);
            this.hopperAngle     = ini.IniReadDouble(IniSectionName, "HopperAngle", 45);
            this.drainHeight     = ini.IniReadDouble(IniSectionName, "DrainHeight", 0.889);
            this.binDiameter     = ini.IniReadDouble(IniSectionName, "BinDiameter", 6.375);
            this.peakOpenning    = ini.IniReadDouble(IniSectionName, "PeakOpenning", 0.838);
            this.roofAngle       = ini.IniReadDouble(IniSectionName, "RoofAngle", 30);
            this.laserOffset     = ini.IniReadDouble(IniSectionName, "LaserOffset", 0);
            this.materialDensity = ini.IniReadDouble(IniSectionName, "MaterialDensity", 599.73);
            this.fillingMode = false;
            this.tankLevelAsDist = 0;

            materialPoly = new Polygon();
            SolidColorBrush orangeBrush = new SolidColorBrush();
            orangeBrush.Color = Color.FromRgb(0xFF, 0x66, 0x00);
            materialPoly.Fill = orangeBrush;
            c.Children.Add(materialPoly);

            SolidColorBrush darkBlueBrush = new SolidColorBrush();
            darkBlueBrush.Color = Color.FromRgb(0x2B, 0x3C, 0x59);

            Polygon tankPoly = new Polygon();
            this.xOffset = x;
            this.yOffset = y;
            tankPoly.Points.Add(new Point(x + 100, y + 0));
            tankPoly.Points.Add(new Point(x + 0, y + 100));
            tankPoly.Points.Add(new Point(x + 0, y + 430));
            tankPoly.Points.Add(new Point(x + 100, y + 530));
            tankPoly.Points.Add(new Point(x + 200, y + 430));
            tankPoly.Points.Add(new Point(x + 200, y + 100));

            tankPoly.Stroke = darkBlueBrush;
            tankPoly.StrokeThickness = 2;
            c.Children.Add(tankPoly);
        }

        public double GetHeight()
        {
            return this.overallHeight;
        }

        public double GetCapacity() // returns metric tons of weight
        {
            double v = GetVolume();
            double kgs = v * this.materialDensity;
            return kgs / 1000;
        }

        public void SetFillingMode(bool mode)
        {
            this.fillingMode = mode;
            DrawTank();
        }
        
        public double GetVolume()
        {
            // Volume of cone = PI * r^2 * h/3
            double hopperVolume = Math.PI * (this.binDiameter / 2) * (this.binDiameter / 2) * (this.hopperHeight - this.drainHeight) / 3;
            // Volume of cylider = PI * r^2 * h
            double binVolume = Math.PI * (this.binDiameter / 2) * (this.binDiameter / 2) * (this.eavesHeight - this.hopperHeight);
            double roofVolume = Math.PI * (this.binDiameter / 2) * (this.binDiameter / 2) * (this.overallHeight - this.eavesHeight) / 3;
            return hopperVolume + binVolume + roofVolume;
        }

        public double GetFilledWeight()
        {
            double v = GetFilledVolume();
            double kgs = v * this.materialDensity;
            return kgs / 1000;
        }
        
        public double GetFilledVolume()
        {
            double distToMaterial = this.tankLevelAsDist;
            double distToBin = this.overallHeight - this.eavesHeight;
            double distToHopper = distToBin + (this.eavesHeight - this.hopperHeight);
            double distToDrain = distToHopper + (this.hopperHeight - this.drainHeight);
            double hopperVolume = 0, binVolume = 0, roofVolume = 0, totalVolume = 0;

            if (distToMaterial <= 0)
                return GetVolume();

            // Height of material pile (this assumes 45 degree pile slope)
            double pileHeight = this.binDiameter / 2;

            if (this.fillingMode)
            {
                if (distToMaterial >= distToDrain)
                {
                    totalVolume = 0;
                }
                if (distToMaterial >= distToHopper)
                {
                    // Top of pile is in hopper, diameter of pile is less than bin diameter, so this will be an approximation
                    // Volume = (full hopper volume) - (volume of cone from top of hopper down to material)
                    totalVolume = getConeVolume(distToDrain - distToHopper);
                    totalVolume -= getConeVolume(distToMaterial - distToHopper);
                }
                else if (distToMaterial >= distToBin)
                {
                    // Material fills hopper and some of bin
                    hopperVolume = getConeVolume(this.hopperHeight - this.drainHeight);
                    binVolume = getCylVolume(distToHopper - distToMaterial);

                    // Volume of anti cone
                    double h = pileHeight;
                    if (h > (distToHopper - distToMaterial))
                        h = distToHopper - distToMaterial;
                    binVolume -= getAntiConeVolume(h);

                    totalVolume = hopperVolume + binVolume;
                }
                else if (distToMaterial < distToBin)
                {
                    // Material fills hopper, some of bin, and some of roof
                    hopperVolume = getConeVolume(this.hopperHeight - this.drainHeight);
                    binVolume = getCylVolume(distToHopper - distToMaterial);

                    // Volume of anti cone
                    double coneFill = 1.0 - distToMaterial / distToBin;
                    double h = pileHeight * (1.0 - coneFill) + distToBin * coneFill;
                    binVolume -= getAntiConeVolume(h);

                    totalVolume = hopperVolume + binVolume;
                }
            }
            else
            {
                // fillingMode is false (draining mode)
                if (distToMaterial >= distToDrain)
                {
                    totalVolume = 0;
                }
                if (distToMaterial >= distToHopper)
                {
                    // Center of pit is below hopper, edges extend into bin
                    // Volume = (full hopper volume) + (partial bin volume) - (pile volume), but 1st and 3rd terms cancel, so...
                    // Volume = partial bin volume = PI * r^2 * (height of edge above bottom of bin)
                    totalVolume = getCylVolume(distToDrain - distToMaterial);
                }
                else if (distToMaterial >= distToBin)
                {
                    // Material fills hopper and some of bin, center of pit is in bin
                    hopperVolume = getConeVolume(this.hopperHeight - this.drainHeight);
                    // Volume of cylider below pit
                    binVolume = getCylVolume(distToHopper - distToMaterial);

                    // Volume of anti cone
                    double h = pileHeight;
                    if (h > (distToMaterial - distToBin))
                        h = distToMaterial - distToBin;
                    binVolume += getAntiConeVolume(h);

                    totalVolume = hopperVolume + binVolume;
                }
                else if (distToMaterial < distToBin)
                {
                    // Material fills hopper, bin, and some of roof
                    hopperVolume = getConeVolume(this.hopperHeight - this.drainHeight);
                    binVolume = getCylVolume(this.eavesHeight - this.hopperHeight);

                    // Roof volume is volume of cone from eaves up to material
                    roofVolume = getConeVolume(distToBin - distToMaterial);
                    totalVolume = hopperVolume + binVolume + roofVolume;
                }
            }
            return totalVolume;
        }

        private double getConeVolume(double h)
        {
            // Volume of cone = PI * r^2 * h/3
            return Math.PI * (this.binDiameter / 2) * (this.binDiameter / 2) * (h / 3);
        }

        private double getCylVolume(double h)
        {
            // Volume of cylider = PI * r^2 * h
            return Math.PI * (this.binDiameter / 2) * (this.binDiameter / 2) * h;
        }

        private double getAntiConeVolume(double h)
        {
            return getCylVolume(h) - getConeVolume(h);
        }

        public void SetTankLevelUsingDistance(double d)     // d = laser measurement in meters
        {
            double max = this.overallHeight - this.drainHeight;
            d -= this.laserOffset; // remove offset here so we never have to worry about it again
            if (d > max) d = max;
            if (d < 0) d = 0;
            this.tankLevelAsDist = d;
            DrawTank();
        }

        public void SetTankLevelUsingPercent(double p)      // p = 0.0 to 100.0
        {
            if (p > 100) p = 100;
            if (p < 0) p = 0;
            this.tankLevelAsDist = ((100.0 - p) / 100.0) * (this.overallHeight - this.drainHeight);
            DrawTank();
        }

        public void DrawTank()
        {
            double yTop = 7;
            double yBottom = 523;
            double yRange = yBottom - yTop;
            double yBinTop = 103;
            double yBinBottom = 427;
            double xLeft = 5;
            double xRight = 195;
            double xMiddle = xLeft + (xRight - xLeft) / 2;

            double yFillTop = yBottom - yRange * (1 - this.tankLevelAsDist / (this.overallHeight - this.drainHeight));
            double yFillEdge;
            if (this.fillingMode)
                yFillEdge = yFillTop + 96;
            else
                yFillEdge = yFillTop - 96;
            if (yFillEdge < yBinTop) yFillEdge = yBinTop;
            if (yFillEdge > yBinBottom) yFillEdge = yBinBottom;

            this.materialPoly.Points.Clear();
            this.materialPoly.Points.Add(new Point(this.xOffset + xMiddle, this.yOffset + yFillTop));
            this.materialPoly.Points.Add(new Point(this.xOffset + xLeft, this.yOffset + yFillEdge));
            this.materialPoly.Points.Add(new Point(this.xOffset + xLeft, this.yOffset + yBinBottom));
            this.materialPoly.Points.Add(new Point(this.xOffset + xMiddle, this.yOffset + yBottom));
            this.materialPoly.Points.Add(new Point(this.xOffset + xRight, this.yOffset + yBinBottom));
            this.materialPoly.Points.Add(new Point(this.xOffset + xRight, this.yOffset + yFillEdge));
        }
    }
}
