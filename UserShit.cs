namespace UserCustom{
    internal class Utilities{
        public static string GetArgumentValue(string InArgument){
            bool founded = false;
            foreach(string arg in Environment.GetCommandLineArgs()){
                if(founded && arg[0] != '-'){
                    founded = false;
                    return arg;
                }
                
                if(arg[0] == '-' && arg.Substring(1,arg.Length - 1) == InArgument){
                    founded = true;
                }
            }
            return "";
        }
    } 
    
    interface Lab{
        public void Entry();
    }
}

