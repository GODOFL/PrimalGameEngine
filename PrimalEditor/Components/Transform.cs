﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Components {
    [DataContract]
    class Transform : Component{
        private Vector3 _position;
        [DataMember]
        public Vector3 Position {
            get => _position;
            set {
                if (_position != value) { 
                    _position = value;
                    OnProperChanged(nameof(Position));
                }
            }
        }

        private Vector3 _rotation;
        [DataMember]
        public Vector3 Rotation {
            get => _rotation;
            set {
                if (_rotation != value) {
                    _rotation = value;
                    OnProperChanged(nameof(Rotation));
                }
            }
        }

        private Vector3 _scale;
        [DataMember]
        public Vector3 Scale {
            get => _scale;
            set {
                if (_scale != value) {
                    _scale = value;
                    OnProperChanged(nameof(Scale));
                }
            }
        }

        public Transform(GameEntity owner) : base(owner) { 
            
        }
    }
}
