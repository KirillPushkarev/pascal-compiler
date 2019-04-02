program p1;
var
   n: array [1..10] of integer;   (* n is an array of 10 integers *)
   i, j: integer;
   s: string;
   r: real;

begin
   (* initialize elements of array n to 0 *)       
   $
   for i := 1 to 10 do
       n[ i ] := i + 100;   (* set element 
	   at location i to i + 100 *)
    (* output each array element's value *)
   
   for j:= 1 to 10 do
      writeln('Element[, j, '] = ', n[j] );

   r := 123.456;
   r := 123..;
   r := 123.456.789;
   r := 12300000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000.01;

   i := 30000;
   i := 100000000000000000000;

   s := 'abc';
   s := '';
   s := 'gshgkjdfhdkjhdfbdajdfadfhmbcadmfhdjgvdfvbdxbmcvbdjafghadfgeyrgfaskhfgeaigfafgadhfgweyfad7tgfdshfggejgidufgkesfgdkahfgbdkfgadfdfgdfdfgeygvfjcwegecejfgcycgajfgef';

   (* not closed comment
end.
