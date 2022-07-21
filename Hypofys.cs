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


namespace VMS.TPS
{
    public class Hypofys
    {
        public void Execute(ScriptContext context)
        {
            // Your code here.
            run(context.IonPlanSetup, context.Patient, context.Course);
            /*Process process = new Process();
            process.StartInfo.FileName = @"\\SKVfile01.skandion.local\Gemensamdata$\Gemensam\Till-folders and personal folders\Erik_Fura\Test_Skandion_210126\WindowsNoEditor\Test_IBA.exe";
            process.Start();
            Thread.Sleep(10000);
            process.WaitForExit();
            MessageBox.Show("hej");*/
        }
        public void run(IonPlanSetup plan, Patient patient, Course kurs)
        {
            CSVwriter csv = new CSVwriter();
            StreamReader sr = new StreamReader(@"\\SKVfile01.skandion.local\Gemensamdata$\Intern\QA Patient\HypofysRegister\HypofysOAR.txt");
            string filename = @"\\SKVfile01.skandion.local\Gemensamdata$\Intern\QA Patient\HypofysRegister\" + plan.Id + "_" + patient.FirstName.Substring(0, 2) + patient.LastName.Substring(0, 2) + patient.Id.Substring(patient.Id.Length - 4) + ".csv";

            DoseValuePresentation dosePresentation = DoseValuePresentation.Absolute;
            VolumePresentation volumePresentation = VolumePresentation.Relative;

            List<string> strOut = new List<string>();
            
            bool ok = false;
            var structures = plan.StructureSet.Structures;
            
            MessageBox.Show("Hej");
            string OAR;
            while (sr.Peek() >= 0)
            {
                OAR = sr.ReadLine();
                if (!string.IsNullOrEmpty(OAR))
                {
                    foreach (var strukt in structures)
                    {
                        if (strukt.Id.Contains(OAR) && !strukt.IsEmpty)
                        {
                            strOut.Add(strukt.Id);
                            ok = true;
                            if (OAR.Contains("CTV") || OAR.Contains("PTV") || OAR.Contains("GTV"))
                            {
                                strOut.Add("Volym cm^3: ;" + strukt.Volume.ToString("0.00"));
                            }
                        }
                        try
                        {
                            if (ok)
                            {
                                strOut.Add("Maxdos: ;" + (plan.GetDVHCumulativeData(strukt, dosePresentation, volumePresentation, 0.001).MaxDose.Dose * 1.1).ToString("0.00"));
                                strOut.Add("Mindos: ;" + (plan.GetDVHCumulativeData(strukt, dosePresentation, volumePresentation, 0.001).MinDose.Dose * 1.1).ToString("0.00") + "");
                                strOut.Add("Medeldos: ;" + (plan.GetDVHCumulativeData(strukt, dosePresentation, volumePresentation, 0.001).MeanDose.Dose * 1.1).ToString("0.00") + "");
                                strOut.Add("Mediandos: ;" + (plan.GetDVHCumulativeData(strukt, dosePresentation, volumePresentation, 0.001).MedianDose.Dose * 1.1).ToString("0.00") + "");
                                //strOut.Add((plan.GetDoseAtVolume(strukt, 2, volumePresentation, dosePresentation).Dose * 1.1).ToString("0.00") + "");
                                strOut.Add("");
                                ok = false;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            try
            {
                Structure Body = structures.FirstOrDefault(strukt => strukt.DicomType.Contains("EXTERNAL"));
                DoseValue GyIsodoseLevel = new DoseValue(15, DoseValue.DoseUnit.Gy);
                DoseValue PercentIsodoseLevel = new DoseValue(95, DoseValue.DoseUnit.Percent);
                strOut.Add("95% isodose - volume: ;" + (plan.GetVolumeAtDose(Body, PercentIsodoseLevel, VolumePresentation.AbsoluteCm3).ToString("0.00")));
                strOut.Add("15 Gy isodose - volume: ;" + (plan.GetVolumeAtDose(Body, GyIsodoseLevel, VolumePresentation.AbsoluteCm3).ToString("0.00")));
            }
            catch
            {

            }

            string message = "";
            foreach (string line in strOut)
            {
                message = message + "\n" + line;
            }
            //MessageBox.Show(message);
            csv.WriteToCSV(filename, strOut);
            MessageBox.Show("Done!");
        }
    }

    class CSVwriter
    {
        public void WriteToCSV(string fileName, List<string> strOut)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
            StreamWriter sw = new StreamWriter(fileName, false);
            foreach (string text in strOut)
            {
                sw.Write(text);
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

    }
}
