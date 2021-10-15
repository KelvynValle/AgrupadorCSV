﻿using System;
using System.Linq;

namespace ClusterCSV.Clusterização.Relacionamento
{
    //classe geral de relações entre dados
    class Relacao
    {
        public object no1;
        public object no2;
        public double distancia;
        public virtual bool Contem(int[] indices)
        {
            return true;
        }
    }

    //classe que inicia uma relação, com apenas um nó.
    //o objetivo é usá-la para mesclar nós iniciais em relações mais complexas
    class RelacaoInicial : Relacao
    {
        //public new int no1;
        public RelacaoInicial(int no1)
        {
            this.no1 = no1;
            this.no2 = null;
        }
        public override bool Contem(int[] indices)
        {
            return indices.Contains((int)no1);
        }
    }

    //classe para representação de relação entre dois nós iniciais
    class RelacaoNoNo : Relacao
    {
        public RelacaoNoNo(RelacaoInicial no1, RelacaoInicial no2, double distancia)
        {
            this.no1 = no1;
            this.no2 = no2;
            this.distancia = distancia;
        }
        public override bool Contem(int[] indices)
        {
            return indices.Contains((int)((RelacaoInicial)no1).no1) || indices.Contains((int)((RelacaoInicial)no2).no1);
        }
    }

    //classe para representação de relação entre grupo e nó individual
    //grupo pode ser uma relação nó-nó, relação grupo-nó ou relação grupo-grupo
    class RelacaoGrupoNo : Relacao
    {
        public RelacaoGrupoNo(Relacao no1, RelacaoInicial no2, double distancia)
        {
            this.no1 = no1;
            this.no2 = no2;
            this.distancia = distancia;
        }
        public override bool Contem(int[] indices)
        {
            bool k = Array.TrueForAll(indices, indice => ((Relacao)no1).Contem(new int[] { indice }));
            return k && indices.Contains((int)((RelacaoInicial)no2).no1);
        }
    }

    //classe para representação de relação entre um grupo e outro
    //grupo pode ser uma relação entre dois nós, relação entre nós e grupos e relação entre grupos e grupos
    class RelacaoGrupoGrupo : Relacao
    {
        public RelacaoGrupoGrupo(Relacao no1, Relacao no2, double distancia)
        {
            this.no1 = no1;
            this.no2 = no2;
            this.distancia = distancia;
        }
        public override bool Contem(int[] indices)
        {
            return Array.TrueForAll(indices, indice => ((Relacao)no1).Contem(new int[] { indice }) || ((Relacao)no2).Contem(new int[] { indice }));
        }
    }
}
