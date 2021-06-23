# RA-ARvore

Trabalho de Conclusão de Curso desenvolvido por Bruno Geisler Vigentas.  
Tem como o objetivo ajudar no conhecimento de folhas de árvores com o auxílio da Realidade Aumentada.  

Utiliza-se dos seguintes pacotes Unity:

* [Barracuda](https://docs.unity3d.com/Packages/com.unity.barracuda@2.0/manual/index.html) - Para detecção das formas de folhas.
* [ARFoundation](https://unity.com/unity/features/arfoundation) - Para parte de Realidade Aumentada.

## Projeto inicial

Foi criado um projeto 3D no Unity HUB, e neste projeto foi feita a instalação dos pacotes do [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/manual/index.html) e do [Barracuda](https://docs.unity3d.com/Packages/com.unity.barracuda@1.0/manual/Installing.html) seguindo suas documentações. Feita a instalação foi criado a cena principal da aplicação, a cena onde estão todos os elementos para o funcionamento da Realidade Aumentada com AR Foundation.  
  
Essa cena conta com os componentes básicos necessários para o funcionamento do AR Roundation, sendo eles o AR Session, que realiza o controle de ciclo de vida da Realidade Aumentada no dispositivo; o AR Session Origin, elemento contendo uma câmera AR e o objetivo de transformar recursos rastreáveis, como superfícies planas e pontos, em sua posição final, orientação e escala na Cena. Além desses elementos principais, foram adicionados uma série de Tracker Managers do AR Foudnation para o objetivo do trabalho específico. Como o Plane Manager, para realizar a detecção de planos no ambiente; o Anchor Manager, para criar e gerenciar as âncoras para os objetos 3D e o Raycast Manager, para realizar os testes de raycast verificando se um determinado ponto intersecta com algum plano.  
  
Tendo os pacotes e a cena configuradas foi então criado os scripts C# para realizarem a detecção e a criação da Realidade Aumentada em sí.

## Treinamento do modelo com dataset personalizado para o Barracuda.

Para se utilizar do Barracuda, é preciso um modelo de machine learning. Para criar esse modelo é necessário um dataset com imagens "etiquetadas" do que se espera detectar, neste caso, os formatos das folhas da árvore. Como não foi possível encontrar um dataset pronto contendo os formatos das folhas necessárias, foi feita a junção de 3 datasets para ter uma variedade maior de imagens, sendo esses datasets o LeafSnap de Kumar et al. (2012), Flavia Dataset de Wu et al. (2007) e o dataset de Chouhan (2019).  
Com as imagens dos datasets selecionadas, iniciou-se o processo de _image labeling_ onde as folhas presentes nas imagens tinham seus formatos "etiquetados" através da ferramenta LabelImg. 
  
Com o dataset etiquetado, se iniciou o treinamento dele através do notebook do google colab criado por [The AI Guy](https://colab.research.google.com/drive/1Mh2HP_Mfxoao6qNFbhfV3u28tG8jAVGk). O notebook realiza o treinamento de um modelo de machine learning utilizando o YOLO a partir de uma série de inputs, como o dataset, as etiquetas das imagens no dataset, e arquivos de configuração do yolo. Ao final, o processo, que pode vir a demorar horas, da como output um modelo de machine learning no formato do yolo, o weights. Como o Barracuda permite apenas modelos no formato onnx, o arquivo de output do notebook é então convertido para onnx através de outro notebook rodando o código do [repositório do usuário zombie0117](https://github.com/zombie0117/yolov3-tiny-onnx-TensorRT). Enfim, após esse passo, o modelo está pronto para ser importado no Barracuda.

## Importação do projeto

- Importe e abra o projeto no Unity 2020.2.5f1.
- Ao abrir o projeto, todas as dependências devem ser resolvidas automaticamente. Entretanto pode ocorrer do pacote do Barracuda não ser encontrado. Neste caso faça o download do pacote neste [LINK](https://drive.google.com/file/d/1oFz3Wp8JN8eiXvkbVCddeuJjX7WS7cG9/view?usp=sharing) e o importe manualmente com os passos abaixo:
  - Descompacte o ZIP baixado.
  - No Unity, vá em Windows -> Package Manager, clique no "+" e selecione a opção "Add package from disk".
  - Vá a pasta onde o ZIP foi descompactado e selecione o arquivo "package.json". Feito o pacote está importado!

## Build do projeto
- Certifique-se que a plataforma de BUILD selecionada é Android, caso não seja, clique no botão "Switch Plataform" para o Android.
- Em "Players Setting", vá nas opções do Android e verifique as seguintes configurações:
  - Write Permissin: External (SDCard)
- Em "XR Plug-in Managament" verifique as seguintes configurações para Android:
  - Initialize XR on Startup: TRUE
  - ARCore: TRUE
  
 - Clique no botão "Build" para gerar uma APK. 



