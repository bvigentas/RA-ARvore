using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafInfos : MonoBehaviour
{

    public static Dictionary<string, Folha> leafs = new Dictionary<string, Folha>();

    public void Start()
    {
        BuildFolha("Palmatifida", "Recebe este nome por parecer uma m�o aberta.", "� presente em �rvores como o Carvalho e o Bordo.");
        BuildFolha("Acicular", "Caracterizada pela forma delgada e fina, como uma agulha.", "� presente em diversas esp�cies de pinheiro como no Pinus Elliottii, no Pinheiro Silvestre e na Araucaria Angustifolia.");
        BuildFolha("Eliptica", "Folha mais larga na por��o mediana.", "� presente em �rvores como a Laranjeira");
        BuildFolha("Multilobada", "Folha que se divide em diversas partes, tamb�m chamados de  l�bolos.", "� presente em alguns tipos de pinheiros, como a Tuia Holandesa.");
        BuildFolha("Bipinada", "Folha composta pinada onde cada um dos fol�olos tamb�m s�o compostos, gerando um padr�o recorrente.", "� presente nas �rvores como Cambu� e Sibipiruna.");
        BuildFolha("Pinulada", "Estrutura composta por v�rios fol�olos(Subdivis�es das folhas). Podendo ser �mpar ou par dependendo da quantidade de fol�olos.", "� presente nas �rvores como Tamarindo e Tipuana.");
        BuildFolha("Linear", "Caracterizada pela forma longa e e comprida.", "� presente nas Gramas e Capins");
        BuildFolha("Flabelada", "Folha em formato de leque.", "� presente em �rvores como a Ginkgo biloba.");
        BuildFolha("Deltoide", "Caracterizada pelo formato triangular.", "� presente no Espinafre da Nova Zel�ndia.");
        BuildFolha("Orbicular", "Caracterizada pelo formato circular.", "A Capuchinha � um exemplo de folha Orbicular.");
        BuildFolha("Romboide", "Caracterizada pelo formato de losango.", "� presenta na �rvore Hibisco.");
        BuildFolha("Trifoliolada", "Caracterizada por ter exatamente 3 fol�olos, todos partindo de um ponto comum.", "A folha de feij�o � um exemplo de Trif�lio.");
    }

    public void BuildFolha(string tipo, string info, string arvores)
    {
        Folha folha = new Folha();
        folha.arvores_folha = arvores;
        folha.informacoes_folha = info;
        folha.tipo_folha = tipo;

        if (!leafs.ContainsKey(tipo.ToLower())) 
        { 
            leafs.Add(tipo.ToLower(), folha);
        }
    }

    public static Folha GetFolha(string tipo)
    {
        return leafs[tipo];
    }

}
