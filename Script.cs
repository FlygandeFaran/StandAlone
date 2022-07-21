using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Diagnostics;
using System.Threading;
using System.IO;


namespace Standalone
{
    public class Script
    {
        private double[] HUcurve = new double[16];
        private double[] relStoppingPwr = new double[16];

        /*public void Execute(ScriptContext context)
        {
            // Your code here.
            run(context.IonPlanSetup, context.Patient, context.Course);
            /*Process process = new Process();
            process.StartInfo.FileName = @"\\SKVfile01.skandion.local\Gemensamdata$\Gemensam\Till-folders and personal folders\Erik_Fura\Test_Skandion_210126\WindowsNoEditor\Test_IBA.exe";
            process.Start();
            Thread.Sleep(10000);
            process.WaitForExit();
            MessageBox.Show("hej");
        }*/
        public void Run(IonPlanSetup plan, Patient patient, Course kurs)
        {
            Image bild = plan.StructureSet.Image;
            Dose dose = plan.Dose;
            Initialize();

            double testHU = 340;

            double testRSP = HUtoRelStopPwr(testHU);

            MessageBox.Show(testRSP.ToString());

            //VVector iso = plan.IonBeams.First().IsocenterPosition;
            //VVector point = new VVector(40, -200, 125);
            //string strOut = "iso x: " + iso.x + " y: " + iso.y + " z: " + iso.z;
            //strOut += "\npoint x: " + (point.x) + " y: " + (point.y) + " z: " + (point.z);
            //strOut += "\npoint x: " + (point.x-iso.x) + " y: " + (point.y- iso.y) + " z: " + (point.z- iso.z);
            ////point -= iso;
            //VVector goal = new VVector(84, -200, 115);
            //VVector nyStart = GantryToDICOM(point, plan.IonBeams.First().IonControlPoints.FirstOrDefault().GantryAngle, plan.IonBeams.First().IonControlPoints.FirstOrDefault().PatientSupportAngle, iso);
            //strOut = strOut + "\npoint x: " + (point.x) + " y: " + (point.y) + " z: " + (point.z);
            //nyStart = DICOMToGantry(nyStart, plan.IonBeams.ElementAt(1).IonControlPoints.FirstOrDefault().GantryAngle, plan.IonBeams.First().IonControlPoints.FirstOrDefault().PatientSupportAngle, iso);
            ////strOut = strOut + "\nDICOMtoGantry x: " + nyStart.x + " y: " + nyStart.y + " z: " + nyStart.z;
            //strOut += "\nGantry angle: " + plan.IonBeams.ElementAt(1).IonControlPoints.FirstOrDefault().GantryAngle;

            //VVector nyPos = NyPosition(point, plan.IonBeams.ElementAt(0).IonControlPoints.FirstOrDefault().GantryAngle, iso) + iso;
            //strOut = strOut + "\nnyPos x: " + nyPos.x + " y: " + nyPos.y + " z: " + nyPos.z;

            //MessageBox.Show(strOut);
        }
        public void Initialize()
        {
            bool ok = true;
            InitiateCalCurve(ok);
        }
        public void InitiateCalCurve(bool ok)
        {
            string[] line;
            TextReader calCurve;
            if (ok)
                calCurve = new StreamReader(@"s_Hjärna.txt");
            else
                calCurve = new StreamReader(@"s_Helkropp.txt");
            for (int i = 0; i < HUcurve.Length; i++)
            {
                line = calCurve.ReadLine().Split(';');
                HUcurve[i] = Convert.ToDouble(line[0]);
                relStoppingPwr[i] = Convert.ToDouble(line[1]);
            }
        }
        public double HUtoRelStopPwr(double HU)
        {
            double roundedDownHU = HUcurve.Where(x => x <= HU).Last();
            int lowerIndex = Array.FindIndex(HUcurve, item => item == roundedDownHU);
            double relStopPwr = relStoppingPwr[lowerIndex] + (HU - HUcurve[lowerIndex]) * (relStoppingPwr[lowerIndex + 1] - relStoppingPwr[lowerIndex]) / (HUcurve[lowerIndex + 1] - HUcurve[lowerIndex]);
            return relStopPwr;
        }
        public VVector DICOMToGantry(VVector point, double gantryInDegrees, double patientSupportInDegrees, VVector isoCenter)
        {
            var retval = point + isoCenter;
            retval = new VVector(retval.x, retval.z, -retval.y);
            retval = RotateZ(retval, -patientSupportInDegrees);
            return RotateY(retval, -gantryInDegrees);
        }
        static VVector NyPosition(VVector start, double gantryAngle, VVector isoCenter)
        {
            start -= isoCenter;
            MessageBox.Show("\npoint x: " + (start.x) + " y: " + (start.y) + " z: " + (start.z));
            double angleInRad = gantryAngle * Math.PI / 180;
            double c1 = start.z;
            double c2 = c1 * Math.Cos(angleInRad);
            double b2 = c2 * Math.Cos(angleInRad);
            double a2 = c2 * Math.Sin(angleInRad);

            return new VVector(a2, start.y, b2);
        }
        public VVector GantryToDICOM(VVector point, double gantryInDegrees, double patientSupportInDegrees, VVector isoCenter)
        {
            //
            // Account for gantry
            //
            var retval = RotateY(point, gantryInDegrees);
            //
            // Account for patient support
            //
            retval = RotateZ(retval, patientSupportInDegrees);
            //
            // Add beam isocenter and reassign axis (HFS patient )
            //
            return new VVector(retval.x, -retval.z, retval.y) + isoCenter;
        }
        public VVector RotateY(VVector point, double angleInDeg)
        {
            var angleInRad = angleInDeg * 2 * Math.PI / 360;
            var c = Math.Cos(angleInRad);
            var s = Math.Sin(angleInRad);
            var x = point.x * c - point.z * s;
            var z = point.x * s + point.z * c;

            return new VVector(x, point.y, z);
        }
        public VVector RotateZ(VVector point, double angleInDeg)
        {
            var angleInRad = angleInDeg * 2 * Math.PI / 360;
            var c = Math.Cos(angleInRad);
            var s = Math.Sin(angleInRad);
            var x = point.x * c - point.y * s;
            var y = point.x * s + point.y * c;
            return new VVector(x, y, point.z);
        }
    }
}
