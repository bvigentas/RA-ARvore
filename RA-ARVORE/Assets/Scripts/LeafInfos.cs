using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafInfos : MonoBehaviour
{

    public static Dictionary<string, Folha> leafs = new Dictionary<string, Folha>();

    public void Start()
    {
        BuildFolha("Espalmada", "Recebe este nome por parecer uma mão aberta.", "É presente em árvores como o Carvalho e o Bordo.");
        BuildFolha("Acicular", "Caracterizada pela forma delgada e fina, como uma agulha.", "É presente em diversas espécies de pinheiro como no Pinus Elliottii, no Pinheiro Silvestre e na Araucaria Angustifolia.");
        BuildFolha("Ovada", "Caracterizada pela forma ovada, como um ovo.", "É presente em árvores como a Laranjeira");
        BuildFolha("Multilobada", "Folha que se divide em diversas partes, também chamados de  lóbolos.", "É presente em alguns tipos de pinheiros, como a Tuia Holandesa.");
        BuildFolha("Bipinulada", "Estrutura de folha composta que se divide em eixos secundários com vários folíolos.", "É presente nas Árvores como Cambuí e Sibipiruna.");
        BuildFolha("Pinulada", "Estrutura composta por vários folíolos(Subdivisões das folhas). Podendo ser ímpar ou par dependendo da quantidade de folíolos.", "É presente nas Árvores como Tamarindo e Tipuana.");
        BuildFolha("Linear", "Caracterizada pela forma longa e e comprida.", "É presente nas Gramas e Capins");
        BuildFolha("Flabelada", "Caracterizada pela forma semi-circular em formato de leque.", "É presente em árvores como a Ginkgo Biloba.");
        BuildFolha("Deltoide", "Caracterizada pelo formato triangular.", "É presente no Espinafre da Nova Zelândia.");
        BuildFolha("Orbicular", "Caracterizada pelo formato circular.", "A Capuchinha é um exemplo de folha Orbicular.");
        BuildFolha("Romboide", "Caracterizada pelo formato de diamante.", "É presenta na árvore Hibisco.");
        BuildFolha("Trofolio", "Estrutura caracterizada por ter exatamente 3 folhas.", "A folha de feijão é um exemplo de Trifólio.");
    }

    public void BuildFolha(string tipo, string info, string arvores)
    {
        Folha folha = new Folha();
        folha.arvores_folha = arvores;
        folha.informacoes_folha = info;
        folha.tipo_folha = tipo;

        leafs.Add(tipo.ToLower(), folha);
    }

    public static Folha GetFolha(string tipo)
    {
        return leafs[tipo];
    }

}
