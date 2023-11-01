﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Utilities {
    public static class Serializer {
        public static void ToFile<T>(T instance, string path) {
            try {
                using var fs = new FileStream(path, FileMode.Create);
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(fs, instance);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);

                //TODO log error
                Logger.Log(MessageType.Error, $"Failed to serialize {instance} to {path}");

                throw;
            }
        }

        internal static T FromFile<T>(string path) {
            try {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(fs);
                return instance;
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);

                //TODO log error
                Logger.Log(MessageType.Error, $"Failed to deserialize {path}");

                throw;
            }
        }
    }
}
