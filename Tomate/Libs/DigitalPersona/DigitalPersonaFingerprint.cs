
using DPFP;

namespace Tomate.Libs.DigitalPersona
{
    public class DigitalPersonaFingerprint : DigitalPersonaCapureHandler
    {
        public static int REGISTER_FINGER_COUNT = 4;
        public bool IsRegister = false;

        private DPFP.Processing.Enrollment? Enroller;
        public delegate void OnRegisterTemplateEventHandler(bool success, DPFP.Template? template);
        public delegate void OnRegisterEventHandler(int counter);

        public event OnRegisterTemplateEventHandler OnRegisterTemplate;
        public event OnRegisterEventHandler OnRegisterCounter;

        public int CounterRegistro = 0;

        public delegate void OnVerifyProccessEventHandler(DPFP.FeatureSet features);
        public event OnVerifyProccessEventHandler OnVerifyProccess;

        public delegate void OnVerifyEventHandler(bool verified);
        public event OnVerifyEventHandler OnVerify;
        private DPFP.Verification.Verification? Verification;

        public byte[]? FingerPrint { get; set; }

        public DigitalPersonaFingerprint()
        {
            this.Init();
            this.Start();
        }

        public void cancelarFingerprint()
        {
            CounterRegistro = 0;
            IsRegister = false;
            FingerPrint = null;
            Enroller = null;
            Verification = null;
        }

        public void registerFingerprint()
        {
            FingerPrint = null;
            CounterRegistro = 0;
            IsRegister = true;
            Enroller = new DPFP.Processing.Enrollment();

        }

        public void CancelRegisterFingerprint()
        {
            IsRegister = false;
            FingerPrint = null;
            CounterRegistro = 0;
            Enroller = null;
        }

        public void validarMultipleFingerprint()
        {
            FingerPrint = null;
            CounterRegistro = 0;
            IsRegister = false;
            Verification = new DPFP.Verification.Verification();
        }

        public void validarFingerprint(byte[]? fingerPrint)
        {
            FingerPrint = fingerPrint;
            CounterRegistro = 0;
            IsRegister = false;
            Verification = new DPFP.Verification.Verification();
        }


        protected override void Init()
        {
            base.Init();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            cancelarFingerprint();
            base.Stop();
        }

        protected override void Process(DPFP.Sample Sample)
        {
            base.Process(Sample);
            if (IsRegister)
            {
                if (Enroller != null)
                {
                    registerProcess(Sample);
                }

            }
            else
            {
                if (Verification != null)
                {
                    verifyProcess(Sample);
                }
            }
        }

        private void registerProcess(DPFP.Sample Sample)
        {
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Enrollment);

            if (features != null)
            {
                try
                {

                    Enroller?.AddFeatures(features);
                    CounterRegistro += 1;
                    OnRegisterCounter(CounterRegistro);
                }
                finally
                {
                    switch (Enroller?.TemplateStatus)
                    {
                        case DPFP.Processing.Enrollment.Status.Ready:
                            OnRegisterTemplate(true, Enroller.Template);
                            validarMultipleFingerprint();
                            break;
                        case DPFP.Processing.Enrollment.Status.Failed:
                            Enroller.Clear();
                            OnRegisterTemplate(false, null);
                            validarMultipleFingerprint();
                            break;
                    }
                }
            }
        }

        private void verifyProcess(DPFP.Sample Sample)
        {
            DPFP.FeatureSet features = ExtractFeatures(Sample, DPFP.Processing.DataPurpose.Verification);
            if (features != null)
            {
                DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();

                if (FingerPrint == null)
                {
                    OnVerifyProccess?.Invoke(features);
                }
                else
                {
                    try
                    {
                        Template t = new Template();
                        t.DeSerialize(FingerPrint);
                        Verification?.Verify(features, t, ref result);
                    }
                    catch { OnVerify?.Invoke(false); return; };
                    if (result.Verified)
                    {
                        OnVerify?.Invoke(true);
                        return;
                    }
                    OnVerify?.Invoke(false);
                }

            }
        }

    }
}
