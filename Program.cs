//entrypoint 
using UserCustom;


try{
    string labnStr = Utilities.GetArgumentValue("labn");
    switch(int.Parse(labnStr)){
        case 1:
            (new Lab1()).Entry();
        break;
        case 2:
             (new Lab2()).Entry();
        break;

        default:

        break;
    }

}catch(Exception ex){
    Console.WriteLine(ex.ToString());
}