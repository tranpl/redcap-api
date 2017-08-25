using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Redcap;
using Redcap.Models;

namespace RedcapApiDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var redcap_api = new RedcapApi("3D57A7FA57C8A43F6C8803A84BB3957B", "http://localhost/redcap/api/");

            var arm1 = new RedcapArm
            {
              arm_num = "2",
              name = "My Test Arm 2"
            };
            var armList = new List<RedcapArm> { };
            armList.Add(arm1);
            var result = redcap_api.DeleteArms(armList, Override.False, RedcapFormat.json, ReturnFormat.json).Result;
            var data = JsonConvert.DeserializeObject(result);
            Console.WriteLine(data);
            Console.ReadLine();

        }
    }
}
