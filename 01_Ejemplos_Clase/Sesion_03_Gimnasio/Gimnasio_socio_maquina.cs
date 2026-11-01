

Producto producto = new Producto("P001", "Teclado", 100000, 10);

Console.WriteLine($"Producto: {producto.Nombre} ({producto.Codigo})");
Console.WriteLine($"Precio sin IVA: {producto.Precio:C}");
Console.WriteLine($"IVA actual: {Producto.PorcentajeIva:P0}");
Console.WriteLine($"Precio final con IVA: {producto.PrecioFinal:C}");
Console.WriteLine($"Inventario disponible: {producto.CantidadDisponible}");

producto.Vender(3);

Console.WriteLine($"Inventario disponible: {producto.CantidadDisponible}");

producto.Vender(100);

Console.WriteLine($"Inventario disponible tras ventas: {producto.CantidadDisponible}");

public class Producto
{
    private static double PorcentajeIva = 0.19;
    public string Codigo { get; private set; }
    public string Nombre { get; private set; }
    public double Precio { get; private set; }
    public int CantidadDisponible { get; private set; }
    public double PrecioFinal { get { return Precio * (1 + PorcentajeIva); } }

    public Producto(string codigo, string nombre, double precio, int cantidadDisponible)
    {
        Codigo = codigo;
        Nombre = nombre;
        Precio = precio;
        CantidadDisponible = cantidadDisponible;
    }   
    
    public void Vender(int cantidad)
    {
        if (cantidad > CantidadDisponible)
        {
            Console.WriteLine("No se puede realizar una venta mayor de la que se tiene en inventario.");
            return;
        }

        CantidadDisponible -= cantidad;
    }
}

// ===================== GIMNASIO=====================//
public class Socio
{   
    private static double Mensualidad = 80000;

    public string Nombre { get; private set; }
    public string Identificacion { get; private set; }
    public double Saldo { get; private set; }
    public bool Activo { get; private set; }

    public Socio(string nombre, string identificacion, double saldo, bool activo)
    {
        Nombre = nombre;
        Identificacion = identificacion;
        Saldo = saldo;
        Activo = activo;
    }

    public void Recargar(double monto)
    {
        Saldo += monto;
    }

    public bool SaldoSuficiente()
    {
        return Saldo >= Mensualidad;
    }

    public void PagarMensualidad()
    {
        if (!SaldoSuficiente())
        {
            Console.WriteLine("Saldo insuficiente para pagar la mensualidad debe recargar más.");
            return;
        }

        Saldo -= Mensualidad;
        Activo = true;
    }
}

public class Maquina
{  
    public static int LimiteHoras { get; private set; } = 100;

    public string Codigo { get; private set; }
    public string Nombre { get; private set; }
    public int HorasUso { get; private set; }
    public bool Disponible { get; private set; }

    public Maquina(string codigo, string nombre, int horasUso, bool disponible)
    {
        Codigo = codigo;
        Nombre = nombre;
        HorasUso = horasUso;
        Disponible = disponible;
    }

    public void RegistrarHoras(int horas)
    {
        HorasUso += horas;
    }

    public bool RequiereMantenimiento()
    {
        return HorasUso >= LimiteHoras;
    }

    public void RealizarMantenimiento()
    {
        HorasUso = 0;
        Disponible = true;
    }
}