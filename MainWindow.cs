using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace Standalone
{
    public partial class MainWindow : Form
    {
        private Course m_course;
        private IonPlanSetup m_plan;
        private Patient m_patient;
        private List<bool> listOfShifts = new List<bool>();
        private List<EvalutationParameters> evalParList;

        public int NumberOfShifts
        {
            get
            {
                int count = 0;
                listOfShifts.Clear();
                ListOfShifts();
                foreach (bool ok in listOfShifts)
                    if (ok)
                        count++;
                return count;
            }
        }

        public static void Main(IonPlanSetup ionplan, Patient patient, Course course)
        {
            System.Windows.Forms.Application.Run(new MainWindow(ionplan, patient, course));
        }
        public MainWindow(IonPlanSetup ionplan, Patient patient, Course course)
        {
            InitializeComponent();
            this.m_plan = ionplan;
            this.m_course = course;
            this.m_patient = patient;
            evalParList = new List<EvalutationParameters>();
            m_patient.BeginModifications();
            InitializeGUI();
        }
        private void InitializeGUI()
        {
            //m_patient.BeginModifications();
            CSItemplate();
            ComboBoxUpdate();
            if (m_plan.IonBeams.Count() == 5)
            {
                cmbFieldBakre.SelectedIndex = 0;
                cmbFieldNedre.SelectedIndex = 4;
                cmbFieldUpper.SelectedIndex = 3;
                cmbFieldExtra.SelectedIndex = 0;
            }
            else
            {
                cmbFieldBakre.SelectedIndex = 0;
                cmbFieldNedre.SelectedIndex = 0;
                cmbFieldUpper.SelectedIndex = 0;
                cmbFieldExtra.SelectedIndex = 0;

            }
        }
        private void ComboBoxUpdate()
        {
            foreach (IonBeam beam in m_plan.IonBeams)
            {
                cmbFieldBakre.Items.Add(beam.Id);
                cmbFieldUpper.Items.Add(beam.Id);
                cmbFieldNedre.Items.Add(beam.Id);
                cmbFieldExtra.Items.Add(beam.Id);
            }
        }
        private void CSItemplate()
        {
            txtYshiftBakre.Text = "2";
            rbPlusMinusBakreY.Checked = true;
            txtXshiftBakre.Text = "2";
            rbPlusMinusBakreX.Checked = true;

            txtYshiftUpper.Text = "3";
            rbPlusMinusUpperY.Checked = true;
            txtXshiftUpper.Text = "5";
            rbPlusMinusUpperX.Checked = true;

            txtYshiftNedre.Text = "10";
            rbPlusMinusNedreY.Checked = true;
            txtXshiftNedre.Text = "5";
            rbPlusMinusNedreX.Checked = true;
        }
        private void ClearAll()
        {
            rbPlusMinusBakreX.Checked = false;
            rbPlusMinusUpperX.Checked = false;
            rbPlusMinusBakreX.Checked = false;
            rbPlusMinusExtraX.Checked = false;

            rbPlusMinusBakreY.Checked = false;
            rbPlusMinusUpperY.Checked = false;
            rbPlusMinusNedreY.Checked = false;
            rbPlusMinusExtraY.Checked = false;

            rbPlusMinusBakreZ.Checked = false;
            rbPlusMinusUpperZ.Checked = false;
            rbPlusMinusNedreZ.Checked = false;
            rbPlusMinusExtraZ.Checked = false;

            rbPlusBakreX.Checked = false;
            rbPlusUpperX.Checked = false;
            rbPlusBakreX.Checked = false;
            rbPlusExtraX.Checked = false;

            rbPlusBakreY.Checked = false;
            rbPlusUpperY.Checked = false;
            rbPlusNedreY.Checked = false;
            rbPlusExtraY.Checked = false;

            rbPlusBakreZ.Checked = false;
            rbPlusUpperZ.Checked = false;
            rbPlusNedreZ.Checked = false;
            rbPlusExtraZ.Checked = false;

            rbMinusBakreX.Checked = false;
            rbMinusUpperX.Checked = false;
            rbMinusBakreX.Checked = false;
            rbMinusExtraX.Checked = false;

            rbMinusBakreY.Checked = false;
            rbMinusUpperY.Checked = false;
            rbMinusNedreY.Checked = false;
            rbMinusExtraY.Checked = false;

            rbMinusBakreZ.Checked = false;
            rbMinusUpperZ.Checked = false;
            rbMinusNedreZ.Checked = false;
            rbMinusExtraZ.Checked = false;

            txtXshiftBakre.Clear();
            txtXshiftUpper.Clear();
            txtXshiftNedre.Clear();
            txtXshiftExtra.Clear();

            txtYshiftBakre.Clear();
            txtYshiftUpper.Clear();
            txtYshiftNedre.Clear();
            txtYshiftExtra.Clear();

            txtZshiftBakre.Clear();
            txtZshiftUpper.Clear();
            txtZshiftNedre.Clear();
            txtZshiftExtra.Clear();
        }
        private void ListOfShifts()
        {
            listOfShifts.Add(rbPlusMinusBakreX.Checked);
            listOfShifts.Add(rbPlusMinusUpperX.Checked);
            listOfShifts.Add(rbPlusMinusBakreX.Checked);
            listOfShifts.Add(rbPlusMinusExtraX.Checked);

            listOfShifts.Add(rbPlusMinusBakreY.Checked);
            listOfShifts.Add(rbPlusMinusUpperY.Checked);
            listOfShifts.Add(rbPlusMinusNedreY.Checked);
            listOfShifts.Add(rbPlusMinusExtraY.Checked);

            listOfShifts.Add(rbPlusMinusBakreZ.Checked);
            listOfShifts.Add(rbPlusMinusUpperZ.Checked);
            listOfShifts.Add(rbPlusMinusNedreZ.Checked);
            listOfShifts.Add(rbPlusMinusExtraZ.Checked);

            listOfShifts.Add(rbPlusBakreX.Checked);
            listOfShifts.Add(rbPlusUpperX.Checked);
            listOfShifts.Add(rbPlusBakreX.Checked);
            listOfShifts.Add(rbPlusExtraX.Checked);

            listOfShifts.Add(rbPlusBakreY.Checked);
            listOfShifts.Add(rbPlusUpperY.Checked);
            listOfShifts.Add(rbPlusNedreY.Checked);
            listOfShifts.Add(rbPlusExtraY.Checked);

            listOfShifts.Add(rbPlusBakreZ.Checked);
            listOfShifts.Add(rbPlusUpperZ.Checked);
            listOfShifts.Add(rbPlusNedreZ.Checked);
            listOfShifts.Add(rbPlusExtraZ.Checked);

            listOfShifts.Add(rbMinusBakreX.Checked);
            listOfShifts.Add(rbMinusUpperX.Checked);
            listOfShifts.Add(rbMinusBakreX.Checked);
            listOfShifts.Add(rbMinusExtraX.Checked);

            listOfShifts.Add(rbMinusBakreY.Checked);
            listOfShifts.Add(rbMinusUpperY.Checked);
            listOfShifts.Add(rbMinusNedreY.Checked);
            listOfShifts.Add(rbMinusExtraY.Checked);

            listOfShifts.Add(rbMinusBakreZ.Checked);
            listOfShifts.Add(rbMinusUpperZ.Checked);
            listOfShifts.Add(rbMinusNedreZ.Checked);
            listOfShifts.Add(rbMinusExtraZ.Checked);
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void btnCSItemplate_Click(object sender, EventArgs e)
        {
            CSItemplate();
        }

        private void btnCalculateDoses_Click(object sender, EventArgs e)
        {
            foreach (Control c in Controls)
            {
                Button b = c as Button;
                if (b != null)
                {
                    b.Enabled = false;
                }
            }
            CalculationPrep();
            pbDoseCalculations.Maximum = evalParList.Count - 1;
            pbDoseCalculations.Step = 1;
            pbDoseCalculations.Value = 0;

            backgroundWorker1.RunWorkerAsync();
            /*CalculationPrep();
            DateTime start = DateTime.Now;
            for (int i = 0; i <= evalParList.Count; i++)
            {
                pbDoseCalculations.Value = i * 100 / evalParList.Count;
                System.Windows.Forms.Application.DoEvents();
                //do calculations
                newEvaluationPlan(evalParList.ElementAt(i));
                if (i == 0)
                {
                    TimeEstimation(start);
                }
            }¨*/
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.WorkerReportsProgress = true;
            DateTime start = DateTime.Now;
            for (int i = 0; i < evalParList.Count; i++)
            {
                this.Invoke(new Action(() =>
                {
                    newEvaluationPlan(evalParList.ElementAt(i));
                }));
                if (i < 1)
                    TimeEstimation(start);
                worker.ReportProgress(i);
            }
            MessageBox.Show("Done!");
            
        }
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbDoseCalculations.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (Control c in Controls)
            {
                Button b = c as Button;
                if (b != null)
                {
                    b.Enabled = true;
                }
            }
        }
        private void TimeEstimation(DateTime start)
        {
            DateTime end = DateTime.Now;
            TimeSpan diff = end - start;
            diff = TimeSpan.FromTicks(diff.Ticks * (NumberOfShifts - 1));
            DateTime answer = end.Add(diff);
            if (this.lblTimeFinish.InvokeRequired)
                this.lblTimeFinish.BeginInvoke((MethodInvoker)delegate () { this.lblTimeFinish.Text = answer.ToString("HH\\:mm"); ; });
            else
                this.lblTimeFinish.Text = answer.ToString("HH\\:mm"); ;
        }
        private void CalculationPrep()
        {
            //Bakre skallfält
            if (!string.IsNullOrEmpty(txtXshiftBakre.Text))
            {
                if (rbPlusBakreX.Checked == true || rbPlusMinusBakreX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtXshiftBakre.Text, cmbFieldBakre.SelectedIndex, Plane.x));
                }
                if (rbMinusBakreX.Checked == true || rbPlusMinusBakreX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtXshiftBakre.Text, cmbFieldBakre.SelectedIndex, Plane.x));
                }
            }
            if (!string.IsNullOrEmpty(txtYshiftBakre.Text))
            {
                if (rbPlusBakreY.Checked == true || rbPlusMinusBakreY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtYshiftBakre.Text, cmbFieldBakre.SelectedIndex, Plane.y));
                }
                if (rbMinusBakreY.Checked == true || rbPlusMinusBakreY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtYshiftBakre.Text, cmbFieldBakre.SelectedIndex, Plane.y));
                }
            }
            if (!string.IsNullOrEmpty(txtZshiftBakre.Text))
            {
                if (rbPlusBakreZ.Checked == true || rbPlusMinusBakreZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtZshiftBakre.Text, cmbFieldBakre.SelectedIndex, Plane.x));
                }
                if (rbMinusBakreZ.Checked == true || rbPlusMinusBakreZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtZshiftBakre.Text, cmbFieldBakre.SelectedIndex, Plane.x));
                }
            }
            //Övre ryggfält
            if (!string.IsNullOrEmpty(txtXshiftUpper.Text))
            {
                if (rbPlusUpperX.Checked == true || rbPlusMinusUpperX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtXshiftUpper.Text, cmbFieldUpper.SelectedIndex, Plane.x));
                }
                if (rbMinusUpperX.Checked == true || rbPlusMinusUpperX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtXshiftUpper.Text, cmbFieldUpper.SelectedIndex, Plane.x));
                }
            }
            if (!string.IsNullOrEmpty(txtYshiftUpper.Text))
            {
                if (rbPlusUpperY.Checked == true || rbPlusMinusUpperY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtYshiftUpper.Text, cmbFieldUpper.SelectedIndex, Plane.y));
                }
                if (rbMinusUpperY.Checked == true || rbPlusMinusUpperY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtYshiftUpper.Text, cmbFieldUpper.SelectedIndex, Plane.y));
                }
            }
            if (!string.IsNullOrEmpty(txtZshiftUpper.Text))
            {
                if (rbPlusUpperZ.Checked == true || rbPlusMinusUpperZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtZshiftUpper.Text, cmbFieldUpper.SelectedIndex, Plane.z));
                }
                if (rbMinusUpperZ.Checked == true || rbPlusMinusUpperZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtZshiftUpper.Text, cmbFieldUpper.SelectedIndex, Plane.z));
                }
            }
            //Nedre ryggfält
            if (!string.IsNullOrEmpty(txtXshiftNedre.Text))
            {
                if (rbPlusNedreX.Checked == true || rbPlusMinusNedreX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtXshiftNedre.Text, cmbFieldNedre.SelectedIndex, Plane.x));
                }
                if (rbMinusNedreX.Checked == true || rbPlusMinusNedreX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtXshiftNedre.Text, cmbFieldNedre.SelectedIndex, Plane.x));
                }
            }
            if (!string.IsNullOrEmpty(txtYshiftNedre.Text))
            {
                if (rbPlusNedreY.Checked == true || rbPlusMinusNedreY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtYshiftNedre.Text, cmbFieldNedre.SelectedIndex, Plane.y));
                }
                if (rbMinusNedreY.Checked == true || rbPlusMinusNedreY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtYshiftNedre.Text, cmbFieldNedre.SelectedIndex, Plane.y));
                }
            }
            if (!string.IsNullOrEmpty(txtZshiftNedre.Text))
            {
                if (rbPlusNedreZ.Checked == true || rbPlusMinusNedreZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtZshiftNedre.Text, cmbFieldNedre.SelectedIndex, Plane.z));
                }
                if (rbMinusNedreZ.Checked == true || rbPlusMinusNedreZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtZshiftNedre.Text, cmbFieldNedre.SelectedIndex, Plane.z));
                }
            }
            //Extra ryggfält
            if (!string.IsNullOrEmpty(txtXshiftExtra.Text))
            {
                if (rbPlusExtraX.Checked == true || rbPlusMinusExtraX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtXshiftExtra.Text, cmbFieldExtra.SelectedIndex, Plane.x));
                }
                if (rbMinusExtraX.Checked == true || rbPlusMinusExtraX.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtXshiftExtra.Text, cmbFieldExtra.SelectedIndex, Plane.x));
                }
            }
            if (!string.IsNullOrEmpty(txtYshiftExtra.Text))
            {
                if (rbPlusExtraY.Checked == true || rbPlusMinusExtraY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtYshiftExtra.Text, cmbFieldExtra.SelectedIndex, Plane.y));
                }
                if (rbMinusExtraY.Checked == true || rbPlusMinusExtraY.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtYshiftExtra.Text, cmbFieldExtra.SelectedIndex, Plane.y));
                }
            }
            if (!string.IsNullOrEmpty(txtZshiftExtra.Text))
            {
                if (rbPlusExtraZ.Checked == true || rbPlusMinusExtraZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("+" + txtZshiftExtra.Text, cmbFieldExtra.SelectedIndex, Plane.z));
                }
                if (rbMinusExtraZ.Checked == true || rbPlusMinusExtraZ.Checked == true)
                {
                    evalParList.Add(new EvalutationParameters("-" + txtZshiftExtra.Text, cmbFieldExtra.SelectedIndex, Plane.z));
                }
            }
        }
        private void newEvaluationPlan(EvalutationParameters evalutationParameters)
        {
            //MessageBox.Show("Hej5");
            /*for (int i = 0; i < 100000000; i++)
            {

            }*/
            string planID = string.Format("{0}F{1}_{2}{3}mm", m_plan.Id.Substring(0, 2), evalutationParameters.Fieldnumber, evalutationParameters.Dir.ToString().ToUpper(), evalutationParameters.Offset.ToString());
            IonPlanSetup IonPlan = (IonPlanSetup)m_course.CopyPlanSetup(m_plan);
            IonBeamParameters parameters = IonPlan.IonBeams.ElementAt(evalutationParameters.Fieldnumber).GetEditableParameters();
            VVector vVector = IonPlan.IonBeams.ElementAt(evalutationParameters.Fieldnumber).IsocenterPosition;
            switch (evalutationParameters.Dir)
            {
                case Plane.x:
                    {
                        vVector.x = vVector.x + evalutationParameters.Offset;
                        break;
                    }
                case Plane.y:
                    {
                        vVector.z = vVector.z + evalutationParameters.Offset;
                        break;
                    }
                case Plane.z:
                    {
                        vVector.y = vVector.y - evalutationParameters.Offset;
                        break;
                    }
            }
            parameters.Isocenter = vVector;
            IonPlan.IonBeams.ElementAt(evalutationParameters.Fieldnumber).ApplyParameters(parameters);
            IonPlan.Id = planID;
            IonPlan.CalculateDose();
        }

    }
    public static class ThreadHelperClass
    {
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = text;
            }
        }
    }
}
