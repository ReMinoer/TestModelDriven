using System.Threading.Tasks;

namespace TestModelDriven.Framework;

public delegate Task AsyncEventHandler();
public delegate Task AsyncEventHandler<in TChange>(TChange change);