using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using DVHextractor;


// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
// [assembly: ESAPIScript(IsWriteable = true)]

namespace Standalone
{
    class Program
    {
        //private readonly System.Timers.Timer t = new System.Timers.Timer(1000);
        static UIListener tba = null;
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                using (VMS.TPS.Common.Model.API.Application app = VMS.TPS.Common.Model.API.Application.CreateApplication())
                {
                    Execute(app);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.Error.WriteLine(e.ToString());

            }
        }
        static void Execute(VMS.TPS.Common.Model.API.Application app)
        {
            //if (patient.Id.Contains("201403200239"))
            // TODO: Add your code here.
            foreach (var patient in app.PatientSummaries)
            {
                if (patient.Id.Contains("201603057793"))
                    Initialize(app.OpenPatientById(patient.Id));
                //if (patient.Id.Contains("196307018645"))
                //if (patient.Id.Contains("Test_m_medullo_HS_8"))
                //if (patient.Id.Contains("19720111TA0I"))
            }
            //CheckVerificationParameters(app.OpenPatientById(patient.Id));

        }

        static void Initialize(Patient patient)
        {
            Course a_course = patient.Courses.FirstOrDefault(s => s.Id.Contains("EF"));
            IonPlanSetup plan = a_course.IonPlanSetups.FirstOrDefault(s => s.Id.Contains("P3_boost"));
            //Course a_course = patient.Courses.FirstOrDefault(s => s.Id.Contains("C1"));
            //IonPlanSetup plan = a_course.IonPlanSetups.FirstOrDefault(s => s.Id.Contains("All"));

            //MessageBox.Show("Hej");


            
            //BordsWET WET = new BordsWET();
            //WET.Execute(patient.StructureSets.First());
            //BordsWETold WETold = new BordsWETold();
            //WETold.Execute(patient.StructureSets.First());
            //DVHextractor.ValidateAndRun.Run(plan);
            //csvWriteTest(plan);
            //Script script = new Script();
            //script.Run(plan, patient, a_course);
            //DepthChoser.Script script = new Script();
            //script.run(plan, patient, a_course);
            //CloseWindowInEclipse();
            //UncertaintyEvaluation(patient, a_course, plan);
            //DVHextractor.MainForm mainWindow = new DVHextractor.MainForm(plan);
            //mainWindow.ShowDialog();
            //System.Windows.Window window = new System.Windows.Window();
            //window.Content = mainWindow;
            //window.Title = "DVH tabell";
            //window.Width = 1500;
            //window.Height = 700;
            //window.ShowDialog();
            //MessageBox.Show("Hej");
            /*VMS.TPS.Hypofys hypofys = new VMS.TPS.Hypofys();
            hypofys.run(plan, patient, a_course);*/

            //DVHunPlan(plan);

            VVector[][] OneSlice = plan.StructureSet.Structures.First().GetContoursOnImagePlane(100);
            StructureChart sc = new StructureChart(OneSlice);
            
            sc.ShowDialog();



        }
        public static void csvWriteTest(IonPlanSetup plan)
        {

            var structures = plan.StructureSet.Structures;
            CSVwriter csv = new CSVwriter();
            string filename = @"\\SKVfile01.skandion.local\Gemensamdata$\Gemensam\MATLAB-verktyg\VerCT\ApprovedAssignedHU.txt";
            StreamReader sr = new StreamReader(filename);
            string OAR;
            bool oarExists = false;
            List<string> listOfAssignedHU = new List<string>();
            foreach (var strukt in structures)
            {
                bool isAssigned = false;
                isAssigned = strukt.GetAssignedHU(out double HUvalue);
                if (isAssigned)
                {
                    while (sr.Peek() >= 0)
                    {
                        OAR = sr.ReadLine();
                        if (!string.IsNullOrEmpty(OAR))
                        {
                            if (strukt.Id.Contains(OAR))
                            {
                                oarExists = true;
                            }
                        }
                    }
                }
                if (!oarExists)
                {
                    DoseValue doseValue = new DoseValue(10, DoseValue.DoseUnit.Percent);
                    try
                    {
                        if (isAssigned && doseValue < plan.Dose.GetDoseToPoint(strukt.CenterPoint))
                        {
                            DialogResult res = MessageBox.Show("Ansatt värde på " + strukt.Id + " är inte samma, vill du lägga till " + strukt.Id + " som undantag?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                            if (res == DialogResult.OK)
                            {
                                listOfAssignedHU.Add(strukt.Id);
                                MessageBox.Show(strukt.Id + " har lagts till som undantag!");
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Kolla att det finns dos i planen");
                    }
                    
                    oarExists = false;
                }

            }
            sr.Close();
            if (listOfAssignedHU.Count > 0)
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    foreach(string assignedHUstructure in listOfAssignedHU)
                    {
                        sw.WriteLine(assignedHUstructure);
                    }
                }
            }


        }
        public static void CloseWindowInEclipse()
        {
            Process[] localAll = Process.GetProcesses();
            var process = Process.GetProcessesByName("ExternalBeam").First();
            int id = Process.GetProcessesByName("ExternalBeam").First().Id;
            //var children = ProcessExtension.GetChildProcesses(process);

            tba = UIListener.Find();

            tba.ListenForPopup();

            //MessageBox.Show("Hej");

            //List<IntPtr> Children = WinAPI.EnumerateProcessWindowHandles(id).ToList();
            //var captions = Children.Select(c => new { Ptr = c, Caption = WinAPI.GetWindowCaption(c), Instructions = WinAPI.GetAllChildrenWindowHandles(c, 10).Select(WinAPI.GetWindowCaption).ToList() }).ToList();

            //var bajsRuta = captions.ElementAt(1);
            //bajsRuta = null;
            //foreach (var caption in captions)
            //{
            //    foreach (var instruction in caption.Instructions)
            //    {
            //        if (instruction.Contains("Field size could"))
            //        {
            //            bajsRuta = caption;
            //        }
            //    }
            //}



            MessageBox.Show("Hej");

            //StreamWriter sr = new StreamWriter(@"G:\Gemensam\Till-folders and personal folders\Erik_Fura\02 Scripts\Script wizard\Projects\Standalone\ProcessList.txt");
            //foreach(Process proc in localAll)
            //{
            //    sr.WriteLine(proc.ProcessName);
            //}


            //MessageBox.Show("Hej");

            //IntPtr hWnd = IntPtr.Zero;
            //int tbaAppId = Process.GetProcessesByName("ExternalBeam").First().Id;




        }
        public static void HUtoWET(IonPlanSetup plan, Patient patient, Course kurs)
        {
            Image bild = plan.StructureSet.Image;
            Dose dose = plan.Dose;

            VVector iso = plan.IonBeams.First().IsocenterPosition;
            VVector point = new VVector(40, -200, 125);
            string strOut = "iso x: " + iso.x + " y: " + iso.y + " z: " + iso.z;
            strOut += "\npoint x: " + (point.x) + " y: " + (point.y) + " z: " + (point.z);
            strOut += "\npoint x: " + (point.x - iso.x) + " y: " + (point.y - iso.y) + " z: " + (point.z - iso.z);
            //point -= iso;
            VVector goal = new VVector(84, -200, 115);

            MessageBox.Show(strOut);
        }
        public static void UncertaintyEvaluation(Patient patient, Course course, IonPlanSetup plan)
        {
            Dose dose = plan.Dose;
            int W, H, D;
            double sx, sy, sz;
            VVector origin, rowDirection, columnDirection;

            W = dose.XSize;
            H = dose.YSize;
            D = dose.ZSize;
            sx = dose.XRes;
            sy = dose.YRes;
            sz = dose.ZRes;
            origin = dose.Origin;
            rowDirection = dose.XDirection;
            columnDirection = dose.YDirection;

            

            List<int[,]> voxels = new List<int[,]>();
            List<PlanUncertainty> uPlans = new List<PlanUncertainty>();
            foreach (var uPlan in plan.PlanUncertainties)
                if (!uPlan.Id.Contains(" "))
                    uPlans.Add(uPlan);

            voxels.Add(new int[dose.XSize, dose.YSize]);
            foreach (var k in uPlans)
                voxels.Add(new int[dose.XSize, dose.YSize]);

            double xsign = rowDirection.x > 0 ? 1.0 : -1.0;
            double ysign = columnDirection.y > 0 ? 1.0 : -1.0;
            double zsign = dose.ZDirection.z;

            double xdiff = Math.Abs(dose.XSize - W);
            double ydiff = Math.Abs(dose.YSize - H);

            //MessageBox.Show("x: " + evalDose.XSize.ToString() + " y: " + evalDose.YSize.ToString() + " z: " + evalDose.ZSize.ToString() +
            //    "\nx: " + W.ToString() + " y: " + H.ToString() + " z: " + D.ToString() +
            //    "\nx: " + xdiff.ToString() + " y: " + ydiff.ToString());

            List<string> strOut = new List<string>();
            strOut.Add("dose (mGy);planID;x;y;z;PatientID");

            int[,] evalVoxelsMin = new int[dose.XSize, dose.YSize]; // skapar en tom dosmatris för VoxelWiseMin
            int[,] evalVoxelsMax = new int[dose.XSize, dose.YSize]; // skapar en tom dosmatris för VoxelWiseMax

            for (int z = 0; z < D; z++) // lopar genom alla planes
            {
                dose.GetVoxels(z, voxels.First()); // hämtar alla voxelvärden från nominella planen
                for (int k = 0; k < voxels.Count - 1; k++) 
                    uPlans.ElementAt(k).Dose.GetVoxels(z, voxels.ElementAt(k+1)); // hämtar alla voxelvärden från robustutvärderingsplanerna en i taget

                for (int y = 0; y < dose.YSize; y++) // loopar genom alla y 
                {
                    for (int x = 0; x < dose.XSize; x++) // loopar genom alla x
                    {
                        List<int> voxelValues = new List<int>();
                        int voxelValueMin = voxels.First()[x, y]; // tar ut voxelvärdet för x och y från nominella planen
                        int voxelValueMax = voxels.First()[x, y];
                        voxelValues.Add(voxels.First()[x, y]);
                        for (int l = 1; l < voxels.Count; l++) // loopar genom alla dosmatriser inklusive nominella planen
                        {
                            voxelValues.Add(voxels.ElementAt(l)[x, y]);
                            if (voxelValueMin > voxels.ElementAt(l)[x, y]) // om värdet är mindre än det tidigare, ersätt
                                voxelValueMin = voxels.ElementAt(l)[x, y];
                            if (voxelValueMax < voxels.ElementAt(l)[x, y]) // om värdet är högre än det tidigare, ersätt
                                voxelValueMax = voxels.ElementAt(l)[x, y];
                        }
                        evalVoxelsMin[x, y] = voxelValueMin; // ansätter det minsta värdet till VoxelWiseMin voxel.
                        evalVoxelsMax[x, y] = voxelValueMax; // ansätter det högsta värdet till VoxelWiseMax voxel.

                        if (voxelValueMax > 10000)
                        {

                        }
                        

                        int x1, y1, z1;
                        x1 = 116;
                        y1 = 62;
                        z1 = 49;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        x1 += 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        x1 += 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        x1 += 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        x1 -= 15;
                        y1 -= 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        y1 -= 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        y1 -= 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        y1 += 15;
                        z1 += 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        z1 += 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                        z1 += 5;
                        if (x == x1 && y == y1 && z == z1)
                            strOut = SaveValues(voxelValues, voxelValueMax, voxelValueMin, x, y, z, strOut);
                    }
                }
            }
            //for (int z = 0; z < D; z++)
            //{

            //    int[,] evalVoxels = new int[dose.XSize, dose.YSize];
            //    dose.GetVoxels(z, voxels.First());
            //    for (int k = 0; k < voxels.Count-1; k++)
            //        uPlans.ElementAt(k).Dose.GetVoxels(z, voxels.ElementAt(k));

            //    for (int y = 0; y < dose.YSize; y++)
            //    {
            //        for (int x = 0; x < dose.XSize; x++)
            //        {
            //            List<int> voxelValues = new List<int>();
            //            int voxelValueMin = voxels.First()[x, y];
            //            int voxelValueMax = voxels.First()[x, y];
            //            voxelValues.Add(voxels.First()[x, y]);
            //            for (int l = 1; l < voxels.Count; l++)
            //            {
            //                voxelValues.Add(voxels.ElementAt(l)[x, y]);
            //                if (voxelValueMin > voxels.ElementAt(l)[x, y]) // om värdet är mindre än det tidigare, ersätt
            //                    voxelValueMin = voxels.ElementAt(l)[x, y];
            //                if (voxelValueMax < voxels.ElementAt(l)[x, y]) // om värdet är högre än det tidigare, ersätt
            //                    voxelValueMax = voxels.ElementAt(l)[x, y];
            //            }
            //            evalVoxelsMin[x, y] = voxelValueMin; // ansätter det minsta värdet till VoxelWiseMin voxel.
            //            evalVoxelsMax[x, y] = voxelValueMax; // ansätter det högsta värdet till VoxelWiseMax voxel.
            //        }
            //    }
            //}
            EvaluationWriter(strOut);
        }
        static List<string> SaveValues(List<int> voxelValues, int voxelValueMax, int voxelValueMin, int x, int y, int z, List<string> strOut)
        {
            strOut.Add(voxelValues.First().ToString() + ";Nominal plan;" + x + ";" + y + ";" + z + ";");
            for (int l = 1; l < voxelValues.Count; l++)
            {
                strOut.Add(voxelValues.ElementAt(l).ToString() + ";U" + l.ToString());
            }
            strOut.Add(voxelValueMin.ToString() + ";Min value");
            strOut.Add(voxelValueMax.ToString() + ";Max value");
            strOut.Add("");
            return strOut;
        }
        public static void EvaluationWriter(List<string> strOut)
        {

            string filename = @"\\SKVfile01.skandion.local\Gemensamdata$\Gemensam\Till-folders and personal folders\Erik_Fura\02 Programmering\C# scripting\Projects\UncertaintyEvaluation\" + "Output" + ".csv";
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            StreamWriter sw = new StreamWriter(filename, false);
            foreach (string text in strOut)
            {
                sw.WriteLine(text);
            }
            sw.Close();
        }

        static void DVHunPlan(IonPlanSetup plan)
        {
            Structure target = plan.StructureSet.Structures.FirstOrDefault(s => s.DicomType.Equals("CTV"));
            DoseValuePresentation doseValue = DoseValuePresentation.Relative;
            VolumePresentation volumePresentation = VolumePresentation.Relative;
            string strOut = "";
            foreach (PlanUncertainty uPlan in plan.PlanUncertainties)
            {
                DVHData dvhData = uPlan.GetDVHCumulativeData(target, doseValue, volumePresentation, 0.01);
                strOut += "\n" + uPlan.Id + " D98: " + DvhExtensions.DoseAtVolume(dvhData, 98).Dose.ToString("0.00");
            }

            MessageBox.Show(strOut);
        }
        static void EvalCSI(Patient _patient)
        {
            Course a_course = _patient.Courses.FirstOrDefault(s => s.Id.Contains("EF"));
            IonPlanSetup plan = a_course.IonPlanSetups.FirstOrDefault(s => s.Id.Contains("P1"));
            Standalone.MainWindow.Main(plan, _patient, a_course);
        }
        static void CheckVerificationParameters(Patient m_patient)
        {
            Course a_course = m_patient.Courses.FirstOrDefault(s => s.Id.Contains("VerPlanEF"));
            IonPlanSetup plan = a_course.IonPlanSetups.FirstOrDefault(s => s.Id.Contains("VerP1"));
            Series m_serie = plan.Series.Study.Series.ToList().ElementAt(1);
            //MessageBox.Show(m_serie.Id);
            string description = plan.Series.Study.Series.ToList().ElementAt(1).Comment;
            //MessageBox.Show(description);
            MessageBox.Show(m_serie.ImagingDeviceId.ToString());
            string calibrationCurve = char.ToUpper(m_serie.ImagingDeviceId.Substring(2)[0]) + m_serie.ImagingDeviceId.Substring(3);
            //MessageBox.Show(calibrationCurve);
            //MessageBox.Show(calibrationCurve);
            //MessageBox.Show(description);
            //if (description.Contains(calibrationCurve))
            //MessageBox.Show(":)");
            string planID = plan.Id.Substring(3, 2);
            //MessageBox.Show(planID);
            int SnoutCounter = 0;
            int MUcounter = 0;
            foreach (Course m_course in m_patient.Courses)
            {
                foreach (IonPlanSetup m_plan in m_course.IonPlanSetups)
                {
                    if (m_plan.ApprovalStatus == PlanSetupApprovalStatus.TreatmentApproved && m_plan.Id.Contains(planID))
                    {
                        for (int i = 0; i < plan.IonBeams.Count(); i++)
                        {
                            string verMU = plan.IonBeams.ElementAt(i).Meterset.Value.ToString();
                            string originalMU = m_plan.IonBeams.ElementAt(i).Meterset.Value.ToString();
                            string verSnoutPos = plan.IonBeams.ElementAt(i).IonControlPoints.First().SnoutPosition.ToString();
                            string originalSnoutPos = m_plan.IonBeams.ElementAt(i).IonControlPoints.First().SnoutPosition.ToString();
                            if (verSnoutPos == originalSnoutPos)
                                SnoutCounter++;
                            if (verMU == originalMU)
                                MUcounter++;
                        }
                    }
                }
            }
            if (SnoutCounter == plan.IonBeams.Count() && MUcounter == plan.IonBeams.Count())
            {
                if (description.Contains(calibrationCurve) || (description.Contains("Hjärna") && calibrationCurve == "Hjarna") || (description.Contains("Hals") && calibrationCurve == "Helkropp"))
                    MessageBox.Show(":)");
                else
                    MessageBox.Show(">:(\n\nKalibreringskurvan och protokollet stämmer inte överens");
            }
            else if (SnoutCounter != plan.IonBeams.Count())
                MessageBox.Show(">:(\n\nSnout positionerna är inte samma får något fält");
            else if (MUcounter != plan.IonBeams.Count())
                MessageBox.Show(">:(\n\nPlanerna har inte samma MU");
        }
        static void run2(Patient patient)
        {
            Structure structure;
            MessageBox.Show(patient.Id);

            Structure strukt = patient.Courses.First().IonPlanSetups.First().StructureSet.Structures.FirstOrDefault(s => s.DicomType.Contains("EXTERNAL"));
            IonBeam beam = patient.Courses.First().IonPlanSetups.First().IonBeams.First();

            


            //MessageBox.Show(beam.IsocenterPosition.x.ToString() + " " + beam.IsocenterPosition.y.ToString() + " " + beam.IsocenterPosition.z.ToString());
            VVector userOrigin = patient.Courses.First().IonPlanSetups.First().StructureSet.Image.UserOrigin;
            MessageBox.Show(userOrigin.x.ToString() + " " + userOrigin.y.ToString() + " " + userOrigin.z.ToString());


            foreach (int triangle in strukt.MeshGeometry.TriangleIndices)


            foreach (var course in patient.Courses)
            {
                structure = course.IonPlanSetups.First().StructureSet.Structures.First();
                if (course.Id.Contains("EF"))
                    MessageBox.Show("Ja");
            }
            
        }
    }
    public class CSVwriter
    {
        public void WriteToCSV(string fileName, List<string> strOut)
        {
            if (File.Exists(fileName))
            {
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
}
