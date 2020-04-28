using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Properties : ScriptableObject
{
    [SerializeField] private Texture icon;

    public Texture Icon { get => icon; set => icon = value; }
}

