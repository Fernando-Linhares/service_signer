using System;

namespace Service.Serialization;

public class State
{
    public Certificate Cert { get; set; }

    public Storage Storage { get; set; }

    public Services[] Services { get; set; }
}