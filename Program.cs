using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lexico_3
{
    class Program
    {
        static void Main(string[] args)
        {
            bool bandera = false;
            try
            {
                using (Lexico lexico = new Lexico("prueba.cpp"))
                {
                    while (!lexico.finArchivo())
                    {
                        lexico.nextToken(bandera);
                    }
                    lexico.log.WriteLine("\n-----------------------------------\n");
                    lexico.log.WriteLine("Líneas del archivo: " + lexico.linea);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }
}