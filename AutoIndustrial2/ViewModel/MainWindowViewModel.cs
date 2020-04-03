using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace AutoIndustrial2
{
    public class MainWindowViewModel : BaseViewModel
    {
        /*
         * A = quarto
         * B = Sala
         * C = Banheiro
         * D = Cozinha
         * E = Lavanderia
         * F = Externa
         * G = quarto (ar)
         * H = sala (ar)
         * I = comando para receber os estados
         */
        #region privates 
        private SerialPort port;
        private readonly int baudRate = 9600;
        private List<string> dados;
        private bool luzSalaChecked;
        private bool luzQuartoChecked;
        private bool arSalaChecked;
        private bool arQuartoChecked;
        private bool luzCozinhaChecked;
        private bool luzLavanderiaChecked;
        private bool luzExternaChecked;
        private bool luzBanheiroChecked;
        #endregion

        #region Propriedades
        public bool LuzQuartoChecked { get => luzQuartoChecked; set => Enviar("A"); }
        public bool LuzSalaChecked { get => luzSalaChecked; set => Enviar("B"); }
        public bool LuzBanheiroChecked { get => luzBanheiroChecked; set => Enviar("C"); }
        public bool LuzCozinhaChecked { get => luzCozinhaChecked; set => Enviar("D"); }
        public bool LuzLavanderiaChecked { get => luzLavanderiaChecked; set => Enviar("E"); }
        public bool LuzExternaChecked { get => luzExternaChecked; set => Enviar("F"); }
        public bool ArQuartoChecked { get => arQuartoChecked; set => Enviar("G"); }
        public bool ArSalaChecked { get => arSalaChecked; set => Enviar("H"); }

        public string QuartoTemperatura { get; set; }
        public string SalaTemperatura { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            port = new SerialPort("COM1", baudRate);
            port.Open();
            //port.DataReceived += Port_DataReceived;

            dados = new List<string>();

            Task.Run(async () =>
            {
                while (true)
                {
                    Enviar("I");

                    Receber();

                    await Task.Delay(500);
                }
            });
        }

        private void Receber()
        {
            while (true)
            {
                if (port.BytesToRead == 0)
                    break;
                string data = port.ReadTo(",");
                dados.Add(data);

                if (dados.Count >= 10)
                {
                    VerificarLista();
                    dados.Clear();
                }
            }
        }

        private bool StringToBoolean(string str)
        {
            return str.Equals("1");
        }

        private void VerificarLista()
        {
            luzQuartoChecked = StringToBoolean(dados[0]);
            OnPropertyChanged(nameof(LuzQuartoChecked));
            luzSalaChecked = StringToBoolean(dados[1]);
            OnPropertyChanged(nameof(LuzSalaChecked));
            luzBanheiroChecked = StringToBoolean(dados[2]);
            OnPropertyChanged(nameof(LuzBanheiroChecked));
            luzCozinhaChecked = StringToBoolean(dados[3]);
            OnPropertyChanged(nameof(LuzCozinhaChecked));
            luzLavanderiaChecked = StringToBoolean(dados[4]);
            OnPropertyChanged(nameof(LuzLavanderiaChecked));
            luzExternaChecked = StringToBoolean(dados[5]);
            OnPropertyChanged(nameof(LuzExternaChecked));
            arQuartoChecked = StringToBoolean(dados[6]);
            OnPropertyChanged(nameof(ArQuartoChecked));
            arSalaChecked = StringToBoolean(dados[7]);
            OnPropertyChanged(nameof(ArSalaChecked));

            double quartoTemp = double.Parse(dados[8].Trim());
            QuartoTemperatura = $"{(80.0 / 1023.0 * quartoTemp).ToString("0.00")} °C";
            double salaTemp = double.Parse(dados[9].Trim());
            SalaTemperatura = $"{((80.0 / 1023.0) * salaTemp).ToString("0.00")} °C";

        }

        private void Enviar(string data)
        {
            if (!port.IsOpen || string.IsNullOrEmpty(data))
                return;

            byte[] buffer = Encoding.Default.GetBytes(data);
            port.Write(buffer, 0, buffer.Length);
        }
    }
}
