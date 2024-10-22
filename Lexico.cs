
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace Lexico_3
{
    public class Lexico : Token, IDisposable
    {
        public StreamReader archivo;
        public StreamWriter log;
        public StreamWriter asm;
        public int linea = 1;

        const int F = -1;

        const int E = -2;

        int[,] TRAND = {
            {0, 1, 2, 3},
            {F, 1, 1, F},
            {F, F, 2, F},
            {F, F, F, F},
        };
        public Lexico()
        {
            log = new StreamWriter("prueba.log");
            asm = new StreamWriter("prueba.asm");
            log.AutoFlush = true;
            asm.AutoFlush = true;
            if (File.Exists("prueba.cpp"))
            {
                archivo = new StreamReader("prueba.cpp");
            }
            else
            {
                throw new Error("El archivo prueba.cpp no existe", log);
            }
        }

        public Lexico(string nombreArchivo)
        {
            string nombreArchivoWithoutExt = Path.GetFileNameWithoutExtension(nombreArchivo);   /* Obtenemos el nombre del archivo sin la extensión para poder crear el .log y .asm */
            if (File.Exists(nombreArchivo))
            {
                log = new StreamWriter(nombreArchivoWithoutExt + ".log");
                asm = new StreamWriter(nombreArchivoWithoutExt + ".asm");
                log.AutoFlush = true;
                asm.AutoFlush = true;
                archivo = new StreamReader(nombreArchivo);
            }
            else if (Path.GetExtension(nombreArchivo) != ".cpp")
            {
                throw new ArgumentException("El archivo debe ser de extensión .cpp");
            }
            else
            {
                throw new FileNotFoundException("La extensión " + Path.GetExtension(nombreArchivo) + " no existe");    /* Defino una excepción que indica que existe un error con el archivo en caso de no ser encontrado */
            }
        }
        public void Dispose()
        {
            archivo.Close();
            log.Close();
            asm.Close();
        }

        private int Columna(char c)
        {
            //int columna;
            if (char.IsWhiteSpace(c))
            {
                return 0;
            }
            else if (char.IsLetter(c)){
                return 1;
            }
            else if (char.IsDigit(c)){
                return 2;
            }
            return 3;
        }
        private void Clasifica(int estado)
        {
            switch (estado)
            {
                case 1: setClasificacion(Tipos.Identificador);  break;
                case 2: setClasificacion(Tipos.Numero); break;
                case 3: setClasificacion(Tipos.Caracter);  break;
            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            int estado = 0;
            while (estado >= 0)
            {
                c = (char)archivo.Peek();
                estado = TRAND[estado, Columna(c)];
                Clasifica(estado);
                if (estado >= 0)
                {
                    archivo.Read();
                    if (c == '\n')
                    {
                        linea++;
                    }
                    if (estado > 0)
                    {
                        buffer += c;
                    }
                    else
                    {
                        buffer = "";
                    }
                }
            }
            if (estado == E)
            {
                if (getClasificacion() == Tipos.Cadena)
                {
                    throw new Error("léxico, se esperaba un cierre de cadena", log, linea);
                }
                else if (getClasificacion() == Tipos.Caracter)
                {
                    throw new Error("léxico, se esperaba un cierre de comilla simple", log, linea);
                }
                else if (getClasificacion() == Tipos.Numero)
                {
                    throw new Error("léxico, se esperaba un dígitos", log, linea);
                }
                else
                {
                    throw new Error("léxico, se espera fin de comentario", log, linea);
                }
            }
            if (!finArchivo())
            {
                setContenido(buffer);
                log.WriteLine(getContenido() + " = " + getClasificacion());
            }
        }
        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }

    }
}

/*

Expresión Regular: Método Formal que a través de una secuencia de caracteres que define un PATRÓN de búsqueda

a) Reglas BNF 
b) Reglas BNF extendidas
c) Operaciones aplicadas al lenguaje

----------------------------------------------------------------

OAL

1. Concatenación simple (·)
2. Concatenación exponencial (Exponente) 
3. Cerradura de Kleene (*)
4. Cerradura positiva (+)
5. Cerradura Epsilon (?)
6. Operador OR (|)
7. Paréntesis ( y )

L = {A, B, C, D, E, ... , Z | a, b, c, d, e, ... , z}

D = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}

1. L.D
    LD
    >=

2. L^3 = LLL
    L^3D^2 = LLLDD
    D^5 = DDDDD
    =^2 = ==

3. L* = Cero o más letras
    D* = Cero o más dígitos

4. L+ = Una o más letras
    D+ = Una o más dígitos

5. L? = Cero o una letra (la letra es optativa-opcional)

6. L | D = Letra o dígito
    + | - = más o menos

7. (L D) L? (Letra seguido de un dígito y al final una letra opcional)

Producción gramátical

Clasificación del Token -> Expresión regular

Identificador -> L (L | D)*

Número -> D+ (.D+)? (E(+|-)? D+)?
FinSentencia -> ;
InicioBloque -> {
FinBloque -> }
OperadorTernario -> ?

Puntero -> ->

OperadorTermino -> + | -
IncrementoTermino -> ++ | += | -- | -=

Término+ -> + (+ | =)?
Término- -> - (- | = | >)?

OperadorFactor -> * | / | %
IncrementoFactor -> *= | /= | %=

Factor -> * | / | % (=)?

OperadorLogico -> && | || | !

NotOpRel -> ! (=)?

Asignación -> =

AsgOpRel -> = (=)?

OperadorRelacional -> > (=)? | < (> | =)? | == | !=

Cadena -> "c*"
Carácter -> 'c' | #D* | Lamda

----------------------------------------------------------------

Autómata: Modelo matemático que representa una expresión regular a través de un GRAFO, 
para una maquina de estado finito, para una máquina de estado finito que consiste en 
un conjunto de estados bien definidos:

- Un estado inicial 
- Un alfabeto de entrada 
- Una función de transición 

*/