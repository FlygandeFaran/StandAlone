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
    public class BordsWET
    {
        private double[] HUcurve = new double[16];
        private double[] relStoppingPwr = new double[16];
        private StructureSet strukturset;
        private string resultat = "";
        private string resultat2 = "";
        //private string resultat3 = "";
        private VariabelSamlingBord bord;
        private VariabelSamlingBild bild;

        public void Execute(StructureSet structureSet)
        {
            strukturset = structureSet;
            run();
        }
        public void run()
        {
            bord = new VariabelSamlingBord();
            bord.HU_bord = -824;
            bild = new VariabelSamlingBild(strukturset);

            ReadCalibrationCurve();
            if (Hittabord())
            {
                string WETstring = "";
                
                double WET_L = Sidoraknaren(1, bord.bord_c[2]);
                double WET_R = Sidoraknaren(-1, bord.bord_c[2]);
                double WET_C = GetWET(0, 215, bord.bord_c[2]) / 10;

                double WET_L_inf = Sidoraknaren(1, bild.dicomorigin[2] + 4 * bild.image_res[2]);
                double WET_R_inf = Sidoraknaren(-1, bild.dicomorigin[2] + 4 * bild.image_res[2]);
                
                double WET_L_sup = Sidoraknaren(1, bild.dicomorigin[2] + (bild.image_size[2] - 5) * bild.image_res[2]);
                double WET_R_sup = Sidoraknaren(-1, bild.dicomorigin[2] + (bild.image_size[2] - 5) * bild.image_res[2]);
                
                WETstring = "Centralsnittet: \nWET vänster = " + WET_L.ToString("0.00") + " cm" + "\n" + "WET höger = " + WET_R.ToString("0.00") + " cm" + "\n" + "WET center = " + WET_C.ToString("0.00") + " cm";
                
                WETstring += "\n\n" + "Inferiort: \nWET vänster = " + WET_L_inf.ToString("0.00") + " cm" + "\n" + "WET höger = " + WET_R_inf.ToString("0.00") + " cm" + "\n\n" + "Superiort: \nWET vänster = " + WET_L_sup.ToString("0.00") + " cm" + "\n" + "WET höger = " + WET_R_sup.ToString("0.00") + " cm";

                if (WET_L > 0 && WET_L <= 1.1 || WET_R > 0 && WET_R <= 1.1 || WET_C > 0 && WET_C <= 1.1 || WET_L_inf > 0 && WET_L_inf <= 1.1 || WET_R_inf > 0 && WET_R_inf <= 1.1 || WET_L_sup > 0 && WET_L_sup <= 1.1 || WET_R_sup > 0 && WET_R_sup <= 1.1)
                    MessageBox.Show(":( \n \nMinst ett WET-värde är under 1.1 cm, kontrollera att bordet inte ligger för långt upp");
                else if (WET_L > 1.2 || WET_R > 1.2 || WET_C > 1.2 || WET_L_inf > 1.2 || WET_R_inf > 1.2 || WET_L_sup > 1.2 || WET_R_sup > 1.2)
                    MessageBox.Show(":( \n \nMinst ett WET-värde är över 1.2 cm, kontrollera att bordet inte ligger för långt ner");
                else
                    MessageBox.Show(":D \n\nBra jobbat! Alla WET-värden är mellan 1.1 cm och 1.2 cm");
                
                //WindowBord.WindowBord wb = new WindowBord.WindowBord(WET_L, WET_R, WET_C, WET_L_inf, WET_R_inf, WET_L_sup, WET_R_sup);
                //wb.ShowDialog();

                MessageBox.Show(WETstring.ToString());
                
            }
        }
        

        private double Sidoraknaren(int sida, double snitt)
        {
            double WET = 0;
            int dist = 215;
            //string distlist = "";
            while (WET == 0 && dist > 0)
            {
                //resultat = "";
                WET = GetWET(sida, dist, snitt) / 10;
                //MessageBox.Show(resultat.ToString());
                //distlist += "\n" + "dist: " + dist.ToString() + ", WET: " + WET.ToString();
                dist = dist - 5;
            }
            //MessageBox.Show(distlist.ToString());
            return WET;
        }

        private bool Hittabord()
        {
            var s = strukturset.Structures;

            foreach (Structure struktur in s)
            {
                double HU = 0;
                struktur.GetAssignedHU(out HU);
                
                if (HU.Equals(bord.HU_bord))
                {
                    bord.bord_struktur = struktur;
                    bord.DefinieraBordCenter(bild);
                }
            }

            if (bord.bord_struktur == null)
            {
                MessageBox.Show("Hittade ingen struktur med HU = -824");
                return false;
            }
            return true;
        }

        private int[,] GetMatris(double snitt)
        {
            int[,] matris = new int[bild.image_size[0], bild.image_size[1]];
            int bord_c_snitt = Convert.ToInt32(Math.Abs((bild.dicomorigin[2] - snitt) / bild.image_res[2]));
            strukturset.Image.GetVoxels(bord_c_snitt, matris);
            return matris;
        }

        public int Pixel(int[,] matris, double lat_flytt)
        {
            int pix_lat_flytt = bord.bord_c_pixel[0] + Convert.ToInt32(Math.Round(lat_flytt / bild.image_res[0]));
            int n = 0;
            int m = 0;
            int bord_kant = 0;
            bool isFOVtoosmall = false;

            for (int i = bord.bord_c_pixel[1]; i >= bord.bord_c_pixel[1] - 50; i--)
            {
                n = i; //Håller koll på vilken pixel vi är på
                
                resultat = resultat + "\n" + "i = " + i.ToString() + ", HU = " + strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, i]) + ", HU-kvot: " + strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, i + 1]) / strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, i]);
                
                if (strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, i + 1]) <= -1000)
                {
                    m += 1;
                    if (m >= 20 / bild.image_res[1])
                    {
                        resultat += "\n" + "FOV är för litet";
                        isFOVtoosmall = true;
                        break;
                    }
                }
                else if (strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, i + 1]) / strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, i]) >= 1.2)
                {
                    break;
                }
            }

            if(isFOVtoosmall)
            {
                bord_kant = 0;
            }
            else
            {
                bord_kant = n - 3; // Härifrån börjar vi räkna WET 
            }
            return bord_kant;
        }

        public double GetWET(int sida, int dist, double snitt)
        {
            int[,] matris = GetMatris(snitt);
            double lat_flytt = dist*sida;
            MessageBox.Show(strukturset.Image.VoxelToDisplayValue(matris[bord.bord_c_pixel[0], bord.bord_c_pixel[1]]).ToString());
            int bord_kant = Pixel(matris, lat_flytt);
            double WET = 0;
            if (bord_kant == 0)
            {
                //MessageBox.Show("FOV är för litet");
            }
            else
            {
                double d = bild.dicomorigin[1] + bord_kant * bild.image_res[1] - bild.image_res[1] / 2;
                double d_small = d;

                double koord_lat_flytt = bord.bord_c[0] + lat_flytt;
                int pix_lat_flytt = bord.bord_c_pixel[0] + Convert.ToInt32(Math.Round(lat_flytt / bild.image_res[0]));

                double steg = 100.0;

                for (int j = bord_kant; j <= bord_kant + Math.Round(70 / bild.image_res[1]) || j < bild.image_size[1]; j++)
                {
                    for (int k = 0; k < steg; k++)
                    {
                        double l = k / steg;
                        d_small = d + l;
                        if (j > bord.bord_c_pixel[1] && !bord.bord_struktur.IsPointInsideSegment(new VVector(koord_lat_flytt, d_small, bord.bord_c[2]))) //mät WET till sista pixeln i bordet
                        {
                            break;
                        }
                        else if (bord.bord_struktur.IsPointInsideSegment(new VVector(koord_lat_flytt, d_small, bord.bord_c[2]))) //pixlarna som är i bordsstrukturen
                        {
                            WET += HUtoRelStopPwr(bord.HU_bord) * bild.image_res[1] * (1 / steg);
                            //resultat2 += "\n" + "WET = " + HUtoRelStopPwr(bord.HU_bord) * bild.image_res[1] * (1 / steg) + ", HU = " + strukturset.Image.VoxelToDisplayValue(matris[bord.bord_c_pixel[0], j]);
                        }
                        else // pixlarna som är ovanför bordsstrukturen
                        {
                            WET += HUtoRelStopPwr(strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, j])) * bild.image_res[1] * (1 / steg);
                            //resultat2 += "\n" + "d_small = " + d_small + " WET = " + HUtoRelStopPwr(strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, j])) * bild.image_res[1] * (1 / steg) + ", HU = " + strukturset.Image.VoxelToDisplayValue(matris[pix_lat_flytt, j]);
                        }
                    }
                    d += bild.image_res[1];
                }
                resultat2 += "\n" + "Totalt WET = " + WET;
            }
            return WET;
        }

        public void ReadCalibrationCurve()
        {
            bool hjarna = true;
            if (strukturset.Image.Series.ImagingDeviceId == "S_hjarna")
                hjarna = true;
            else if (strukturset.Image.Series.ImagingDeviceId == "S_helkropp")
                hjarna = false;
            else
                MessageBox.Show("Fel kalibreringskurva");
            
            string[] line;
            TextReader calCurve;
            if (hjarna)
                calCurve = new StreamReader(@"\\SKVfile01.skandion.local\Gemensamdata$\Gemensam\Till-folders and personal folders\Linnea Lund\Scripts\BordsWET\BordsWET\s_Hjärna.txt");
            else
                calCurve = new StreamReader(@"\\SKVfile01.skandion.local\Gemensamdata$\Gemensam\Till-folders and personal folders\Linnea Lund\Scripts\BordsWET\BordsWET\s_Helkropp.txt");
            //calCurve = new StreamReader(@"\\Client\G$\Gemensam\Till-folders and personal folders\Linnea Lund\Scripts\BordsWET\BordsWET\s_Helkropp.txt");
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
    }
    public class VariabelSamlingBord
    {
        public VariabelSamlingBord()
        {

        }

        public Structure bord_struktur { get; set; }
        public double[] bord_c { get; set; }
        public int[] bord_c_pixel { get; set; }
        public int HU_bord { get; set; }

        public void DefinieraBordCenter(VariabelSamlingBild bild)
        {
            bord_c = new double[] { bord_struktur.CenterPoint.x, bord_struktur.CenterPoint.y, bord_struktur.CenterPoint.z };
            bord_c_pixel = new int[] { Math.Abs(Convert.ToInt32(Math.Round((Math.Abs(bild.dicomorigin[0] - bord_c[0])) / bild.image_res[0]))), Math.Abs(Convert.ToInt32(Math.Round((Math.Abs(bild.dicomorigin[1] - bord_c[1])) / bild.image_res[1]))) };
        }
        
    }

    public class VariabelSamlingBild
    {
        public VariabelSamlingBild(StructureSet Strset)
        {
            strukturset = Strset;
            Definierabild();
        }
        
        private StructureSet strukturset;

        public double[] dicomorigin { get; set; }
        public double[] image_res { get; set; }
        public int[] image_size { get; set; }
        public double[] center { get; set; }

        public void Definierabild()
        {
            dicomorigin = new double[] { strukturset.Image.Origin.x, strukturset.Image.Origin.y, strukturset.Image.Origin.z }; //DICOM-koordinater för mittpunkten i den översta voxeln längst till vänster i första bildplanet [x,y,z]
            image_res = new double[] { strukturset.Image.XRes, strukturset.Image.YRes, strukturset.Image.ZRes }; //Bildens upplösning i mm [x,y,z]
            image_size = new int[] { strukturset.Image.XSize, strukturset.Image.YSize, strukturset.Image.ZSize }; //Bildens storlek i voxlar [x,y,z]
            center = new double[] { dicomorigin[0] + image_res[0] * image_size[0] / 2 - image_res[0] / 2, dicomorigin[1] + image_res[1] * image_size[1] / 2 - image_res[1] / 2, dicomorigin[2] + image_res[2] * image_size[2] / 2 - image_res[2] / 2 };
        }
    }
}

