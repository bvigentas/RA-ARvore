using System.Net;
using System;
using System.IO;
using UnityEngine;

public class FolhaService : MonoBehaviour
{
    public Folha getArvoreInfo(string nomeCientifico)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://apirvore.herokuapp.com/api/folha/{0}", nomeCientifico));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            Folha arvoreInfo = JsonUtility.FromJson<Folha>(jsonResponse);
            return arvoreInfo;
        }
        catch (WebException e)
        {
            var folha = new Folha();
            folha.arvores_folha = "";
            folha.informacoes_folha = "";
            folha.tipo_folha = "";

            return folha;
        }
    }
}
