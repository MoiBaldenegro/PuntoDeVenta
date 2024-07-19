using DPFP.Capture;
using System;
using System.Diagnostics;
using System.Drawing;

namespace Tomate.Libs.DigitalPersona
{
    public class DigitalPersonaCapureHandler : DPFP.Capture.EventHandler
    {

        public delegate void OnDeviceChange(bool available);
        public event OnDeviceChange? OnDeviceChangeEvent;

        private bool _available = false;


        private bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
                if (OnDeviceChangeEvent != null)
                {
                    OnDeviceChangeEvent(Available);
                }

            }

        }

        private DPFP.Capture.Capture? Capture;

        protected virtual void Init()
        {
            try
            {
                Capture = new DPFP.Capture.Capture();

                if (Capture != null)
                {
                    Capture.EventHandler = this;
                }


            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        public bool isAvailable()
        {
            return Available;
        }

        protected virtual void Process(DPFP.Sample Sample)
        {

            //DrawPicture(ConvertSampleToBitmap(Sample));
        }

        public virtual void Start()
        {
            if (Capture != null)
            {
                try
                {
                    Capture.StartCapture();

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
            }
        }

        public virtual void Stop()
        {
            if (null != Capture)
            {
                try
                {
                    Capture.StopCapture();
                }
                catch
                {

                }
            }
        }

        public virtual void Dispose()
        {
            if (null != Capture)
            {
                try
                {
                    Capture.StopCapture();
                    Capture.Dispose();
                    Capture = null;
                }
                catch
                {

                }
            }
        }


        #region EventHandler Members:

        public void OnComplete(object Capture, string ReaderSerialNumber, DPFP.Sample Sample)
        {
            Process(Sample);
        }

        public void OnFingerGone(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnFingerTouch(object Capture, string ReaderSerialNumber)
        {

        }

        public void OnReaderConnect(object Capture, string ReaderSerialNumber)
        {
            Available = true;
        }

        public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
        {
            Available = false;
        }

        public void OnSampleQuality(object Capture, string ReaderSerialNumber, DPFP.Capture.CaptureFeedback CaptureFeedback)
        {

        }
        #endregion

        protected Bitmap ConvertSampleToBitmap(DPFP.Sample Sample)
        {
            DPFP.Capture.SampleConversion Convertor = new DPFP.Capture.SampleConversion();
            Bitmap? bitmap = null;
            Convertor.ConvertToPicture(Sample, ref bitmap);
            return bitmap;
        }

        protected DPFP.FeatureSet? ExtractFeatures(DPFP.Sample Sample, DPFP.Processing.DataPurpose Purpose)
        {
            DPFP.Processing.FeatureExtraction Extraction = new DPFP.Processing.FeatureExtraction();
            DPFP.Capture.CaptureFeedback feedback = DPFP.Capture.CaptureFeedback.None;
            DPFP.FeatureSet features = new DPFP.FeatureSet();
            Extraction.CreateFeatureSet(Sample, Purpose, ref feedback, ref features);
            if (feedback == DPFP.Capture.CaptureFeedback.Good)
                return features;
            else
                return null;
        }
    }
}
