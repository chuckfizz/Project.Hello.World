using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.IO;

namespace Hello.World
{
    public interface IHello
    {
        void TellThePeople(String Input);
    }
    public class Hello : IHello
    {
        public void TellThePeople(String Input)
        {
            Console.WriteLine(Input);
        }
     }
    public interface IWorld
    {
    }
    public class World : IWorld
    {
        private IHello _Hello;
        [Dependency]
        public IHello Hello
        {
            get { return _Hello; }
            set { _Hello = value; }
        }
        public void TellThePeople(String Input)
        {
            _Hello.TellThePeople(Input);
        }
    }
    public interface IDataAccess
    {
        string GetSomeData();
    }

    public class SqlDataAccess : IDataAccess
    {
        public string GetSomeData()
        {
            return "Data will come from a Sql Database";
        }
    }

    public class OracleDataAccess : IDataAccess
    {
        public string GetSomeData()
        {
            return "Data will come from a Oracle Database";
        }
    }
    public class WebServiceAccess : IDataAccess
    {
        public string GetSomeData()
        {
            return "Data will come from a Web Service";
        }
    }
    public class DataController
    {
        IDataAccess _dataAccess;

        public DataController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Dependency]
        public IDataAccess DataAccess
        {
            get { return _dataAccess; }
            set { _dataAccess = value; }
        }

        public void PrintSomeData()
        {
            Console.WriteLine(_dataAccess.GetSomeData());
        }

    }

    /// <summary>
    /// Purpose: 1) Register Interfaces and Concrete classes
    ///          2) Resolve the classes
    ///          3) Send data to a device
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            string _Device = ConfigurationManager.AppSettings["Device"];
            string _Postiing = ConfigurationManager.AppSettings["Posting"];
            IUnityContainer unitycontainer = new UnityContainer();
            unitycontainer.RegisterType<IHello, Hello>();
            unitycontainer.RegisterType<IDataAccess, SqlDataAccess>();
            World hello = unitycontainer.Resolve<World>();
            SqlDataAccess controller = unitycontainer.Resolve<SqlDataAccess>();
            switch (_Device)
            {
                case "Console":
                    hello.TellThePeople("Hello World" + " :: " + controller.GetSomeData());
                    break;
                case "Posting":
                    /*  Instanciate a utility for posting -- something like the following:
                     *  
                     *  string json = "{\"Hello\":\"World\"," + "\"Data\":\"controller.GetSomeData()\"}";
                     *  HelperPostClass Posting = new HelperPostClass()
                     *  Posting.Url = _Postiing;
                     *  Posting.Data = json;
                     *  Posting.Send;
                     * 
                     */
                    break;
                default:
                    hello.TellThePeople("Hello World" + " :: " + controller.GetSomeData());
                    break;
            }
            Console.ReadLine();
        }
    }
}
