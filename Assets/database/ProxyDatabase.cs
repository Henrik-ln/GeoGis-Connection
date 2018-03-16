using Database.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ProxyDatabase
{
    private int id;
    private MediatorDatabase mediatorDatabase;

    public ProxyDatabase(MediatorDatabase mediatorDatabase)
    {
        this.mediatorDatabase = mediatorDatabase;
        this.id = mediatorDatabase.getNewId();
    }

    public int Id { get { return id; } set { id = value; } }
    public MediatorDatabase MediatorDatabase { get { return mediatorDatabase; } set { mediatorDatabase = value; } }

    public virtual Dictionary<String, List<Object>> getObjectLists()
    {
        return null;
    }
}
