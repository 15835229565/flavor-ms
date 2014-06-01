using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Flavor.Common {
    class PortLevel {
        public enum PortStates {
            Closed,
            Opened,
            Closing,
            Opening,
            ErrorClosing,
            ErrorOpening
        }
        public class ByteReceivedEventArgs: EventArgs {
            readonly byte b;
            public byte Byte {
                get { return b; }
            }
            public ByteReceivedEventArgs(byte b) {
                this.b = b;
            }
        }
        public delegate void ByteReceivedDelegate(object sender, ByteReceivedEventArgs e);
        public event ByteReceivedDelegate ByteReceived;
        protected void OnByteReceived(byte b) {
            if (ByteReceived != null)
                ByteReceived(this, new ByteReceivedEventArgs(b));
        }
        public class BytesReceivedEventArgs: EventArgs {
            private readonly byte[] bytes;
            public byte[] Bytes {
                get { return bytes; }
            }
            private readonly int count;
            public int Count {
                get { return count; }
            }
            public BytesReceivedEventArgs(byte[] bytes, int count) {
                this.bytes = bytes;
                this.count = count;
            }
        }
        public delegate void BytesReceivedDelegate(object sender, BytesReceivedEventArgs e);
        public event BytesReceivedDelegate BytesReceived;
        protected void OnBytesReceived(byte[] bytes) {
            OnBytesReceived(bytes, bytes.Length);
        }
        protected void OnBytesReceived(byte[] bytes, int count) {
            if (BytesReceived != null)
                BytesReceived(this, new BytesReceivedEventArgs(bytes, count));
        }
        // TODO: use in Commander and higher
        public class ErrorPortEventArgs {
            private readonly bool isSevere;
            public bool IsSevere {
                get { return isSevere; }
            }
            private readonly string message;
            public string Message {
                get { return message; }
            }
            public ErrorPortEventArgs(bool isSevere, string message) {
                this.isSevere = isSevere;
                this.message = message;
            }
        }
        public delegate void ErrorPortDelegate(object sender, ErrorPortEventArgs e);
        public event ErrorPortDelegate ErrorPort;
        protected void OnErrorPort(bool isSevere, string message) {
            if (ErrorPort != null)
                ErrorPort(this, new ErrorPortEventArgs(isSevere, message));
        }
        SerialPort serialPort = null;
        void Receiving() {
            serialPort.DataReceived += _serialPort_DataReceived;
        }
        void StopReceiving() {
            serialPort.DataReceived -= _serialPort_DataReceived;
        }
        void _serialPort_DataReceived(object sender, EventArgs e) {
            SerialPort port = sender as SerialPort;
            List<byte> bytes = new List<byte>(port.BytesToRead);
            while (port.IsOpen && port.BytesToRead > 0) {
                byte ch;
                try {
                    ch = (byte)port.ReadByte();
                } catch {
                    OnErrorPort(false, "Error(reading byte)");
                    continue;
                }
                OnByteReceived(ch);
                bytes.Add(ch);
            }
            OnBytesReceived(bytes.ToArray());
        }
        // NOW not used..
        void SerialPortDataReceived(object sender, EventArgs e) {
            SerialPort port = sender as SerialPort;
            int n = port.BytesToRead;
            byte[] bytes = new byte[n];
            if (port.IsOpen) {
                try {
                    int actual_n = port.Read(bytes, 0, n);
                    if (actual_n != n) {
                        OnErrorPort(false, "Problem(reading bytes)");
                    }
                    OnBytesReceived(bytes, actual_n);
                } catch {
                    OnErrorPort(false, "Error(reading bytes)");
                }
            }
        }
        public static string[] AvailablePorts {
            get { return SerialPort.GetPortNames(); }
        }
        public PortStates Open(string name, int baudrate) {
            if (serialPort != null) {
                if (serialPort.IsOpen) {
                    OnErrorPort(false, "Port already opened");
                    return PortStates.Opened;
                }
                serialPort.Dispose();
            }
            serialPort = new SerialPort();
            serialPort.PortName = name;
            serialPort.BaudRate = baudrate;
            serialPort.DataBits = 8;
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.StopBits = System.IO.Ports.StopBits.One;
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 1000;

            try {
                serialPort.Open();
            } catch (Exception Error) {
                OnErrorPort(true, Error.Message);
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка обращения к последовательному порту");
                return PortStates.ErrorOpening;
            }
            Receiving();
            return PortStates.Opening;
        }
        public PortStates Close() {
            if (serialPort == null) {
                OnErrorPort(true, "Порт не инициализирован");
                System.Windows.Forms.MessageBox.Show("Порт не инициализирован", "Ошибка обращения к последовательному порту");
                return PortStates.ErrorClosing;
            }
            if (!serialPort.IsOpen) {
                OnErrorPort(false, "Port already closed");
                return PortStates.Closed;
            }
            try {
                StopReceiving();
                serialPort.Close();
                serialPort.Dispose();
                serialPort = null;
            } catch (Exception Error) {
                OnErrorPort(true, Error.Message);
                System.Windows.Forms.MessageBox.Show(Error.Message, "Ошибка обращения к последовательному порту");
                return PortStates.ErrorClosing;
            }
            return PortStates.Closing;
        }
        public void Send(byte[] message) {
            try {
                serialPort.Write(message, 0, message.Length);
            } catch {
                OnErrorPort(false, "Error writing this command to serial port:");
            }
        }
    }
}
