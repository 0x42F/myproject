﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleServerTCP_soket {
    class Program {
        static void Main(string[] args) {

            Console.OutputEncoding = Encoding.UTF8;

            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint iPEndPoint = new IPEndPoint(ipAddr, 11000);

            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Назначаем сокет локальной конечной точке и слушаем входящие сокеты
            try {

                sListener.Bind(iPEndPoint);
                sListener.Listen(10);

                // Начинаем слушать соединения
                while (true) {

                    Console.WriteLine($"Ожидаем соединение через порт {iPEndPoint}");

                    // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;
                    // Мы дождались клиента, пытающегося с нами соединиться

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "\n\n");

                    // Отправляем ответ клиенту\
                    string reply = "Спасибо за запрос в " + data.Length.ToString()
                            + " символов";

                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if(data.IndexOf("<TheEnd>") > -1) {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            } finally {
                Console.ReadLine();
            }
            Console.ReadKey();
        }
    }
}
