//entrypoint 
using UserCustom;
using Lab2;

try{
    string labnStr = Utilities.GetArgumentValue("labn");
    switch(int.Parse(labnStr)){
        case 1:
            (new Lab1()).Entry();
        break;
        case 2:
             (new Lab2.Lab2()).Entry();
        break;

        default:

        break;
    }

}catch(Exception ex){
    Console.WriteLine(ex.ToString());
}