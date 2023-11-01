using PrimalEditor.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrimalEditor.Components {
    interface IMSComponent { 
    
    }

    [DataContract]
    abstract class Component : ViewModelBase{
        [DataMember]
        public GameEntity Owner { set; private get; }

        public Component(GameEntity owner) { 
            Debug.Assert(owner != null);
            Owner = owner;
        }
    }

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : Component{ 
        
    }
}


//Component是null文件读不进去