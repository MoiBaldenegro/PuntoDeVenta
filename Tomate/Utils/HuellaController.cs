using DPFP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tomate.Libs.DigitalPersona;
using Tomate.Models.Usuarios;

namespace Tomate.Utils
{
    public class HuellaController
    {

        public delegate void OnHuellaRegistroHandler(bool success, byte[]? huella);
        public delegate void OnHuellaRegistroCounterHandler(int counter, int total);
        public delegate void OnHuellaValidarHandler(bool success, UsuarioHuella? folio);
        public delegate void OnHuellaValidarProgresoHandler();
        public delegate void OnDeviceChange(bool available);

        public event OnHuellaRegistroHandler OnHuellaRegistroEvent;
        public event OnHuellaRegistroCounterHandler OnHuellaRegistroCounterEvent;
        public event OnHuellaValidarHandler OnHuellaValidarEvent;
        public event OnHuellaValidarProgresoHandler OnHuellaValidarProgresoEvent;
        public event OnDeviceChange OnDeviceChangeEvent;

        public DigitalPersonaFingerprint DigitalPersonaFingerprint { get; set; }

        public string FingerprintSDK;
        public bool Available = false;
        private List<UsuarioHuella> Huellas { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public HuellaController()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public void InitSDK()
        {
            //Comprueba si esta instalado el SDK, si no se hace esa comprobacion se cierra el programa
            if (SoftwareUtils.IsSoftwareInstalled("DigitalPersona"))
            {
                new Task(() =>
                {
                    try
                    {
                        InitDigitalPersona();
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.Message);
                    }

                }).Start();
            }
        }



        private void InitDigitalPersona()
        {
            DigitalPersonaFingerprint = new DigitalPersonaFingerprint();
            DigitalPersonaFingerprint.OnRegisterTemplate += DigitalPersonaRegistrerFinger;
            DigitalPersonaFingerprint.OnRegisterCounter += DigitalPersonaRegisterCounter;
            DigitalPersonaFingerprint.OnVerifyProccess += DigitalPersonaVerifyedMultiple;
            DigitalPersonaFingerprint.OnVerify += DigitalPersonaVerifyed;
            DigitalPersonaFingerprint.OnDeviceChangeEvent += OnDeviceChangeFingerprintEvent;
        }

        public void OnDeviceChangeFingerprintEvent(bool available)
        {
            Available = available;
            OnDeviceChangeEvent(available);
        }

        public void Start()
        {
            DigitalPersonaFingerprint.Start();
        }

        public void Stop()
        {
            DigitalPersonaFingerprint.Stop();
        }

        public void Cancelar()
        {
            DigitalPersonaFingerprint.cancelarFingerprint();
        }

        public int getTotalHuella()
        {
            return DigitalPersonaFingerprint.REGISTER_FINGER_COUNT;
        }

        public void DetenerRegistrarHuella()
        {
            if (DigitalPersonaFingerprint.IsRegister)
            {
                DigitalPersonaFingerprint.CancelRegisterFingerprint();
            }
        }

        public bool registrarHuella()
        {
            if (!Available)
            {
                return false;
            }
            DigitalPersonaFingerprint.registerFingerprint();
            return true;
        }

        public bool validarMultipleHuella(List<UsuarioHuella> huellas)
        {
            if (!Available)
            {
                return false;
            }
            Huellas = huellas;
            if (!DigitalPersonaFingerprint.IsRegister)
            {
                DigitalPersonaFingerprint.validarMultipleFingerprint();
            }

            return true;
        }

        public bool validarHuella(byte[] huella)
        {
            if (!Available)
            {
                return false;
            }
            DigitalPersonaFingerprint.validarFingerprint(huella);
            return true;
        }

        private void DigitalPersonaVerifyedMultiple(DPFP.FeatureSet features)
        {
            OnHuellaValidarProgresoEvent();
            var Verification = new DPFP.Verification.Verification();

            if (Huellas != null)
            {
                foreach (UsuarioHuella huella in Huellas)
                {
                    DPFP.Verification.Verification.Result result = new DPFP.Verification.Verification.Result();

                    try
                    {
                        Template t = new Template();
                        t.DeSerialize(huella.Huella);
                        Verification?.Verify(features, t, ref result);
                    }
                    catch
                    {
                        Debug.Print($"Error al validar huella {huella.Id}");
                    };
                    if (result.Verified)
                    {
                        OnHuellaValidarEvent(true, huella);
                        return;
                    }
                }
            }


            OnHuellaValidarEvent(true, null);
        }


        private void DigitalPersonaVerifyed(bool status)
        {

            OnHuellaValidarEvent(status, null);
            if (status)
            {
                DigitalPersonaFingerprint.cancelarFingerprint();
            }
        }

        private void DigitalPersonaRegistrerFinger(bool success, DPFP.Template? template)
        {
            OnHuellaRegistroEvent(success, template?.Bytes);
            DigitalPersonaFingerprint.cancelarFingerprint();
        }

        private void DigitalPersonaRegisterCounter(int counter)
        {
            OnHuellaRegistroCounterEvent(counter, DigitalPersonaFingerprint.REGISTER_FINGER_COUNT);
        }
    }
}
