# ChugSharp

[![](https://img.shields.io/github/license/sblmnl/ChugSharp?color=red&style=flat-square)](LICENSE)
[![](https://img.shields.io/github/workflow/status/sblmnl/ChugSharp/.NET?style=flat-square)](https://github.com/sblmnl/ChugSharp/actions/workflows/dotnet.yml)
[![](https://img.shields.io/nuget/v/ChugSharp?style=flat-square)](https://www.nuget.org/packages/ChugSharp)
[![](https://img.shields.io/nuget/dt/ChugSharp?color=blue&label=nuget&style=flat-square)](https://www.nuget.org/packages/ChugSharp)
[![](https://img.shields.io/github/v/release/sblmnl/ChugSharp?color=lightgray&style=flat-square)](https://github.com/sblmnl/ChugSharp/releases)

The official implementation of the [Chug](https://github.com/sblmnl/chug) encryption and padding algorithms.

## Getting Started

### Installation

```
Install-Package ChugSharp
```

### Sample Code

```csharp
using ChugSharp;
using System;
using System.Text;

class Program
{
    static void Main()
    {
        var chug = new Chug();

        byte[] key = Encoding.UTF8.GetBytes("My super secret encryption key");
        byte[] plaintext = Encoding.UTF8.GetBytes("My secret message");
        byte[] ciphertext = chug.Encrypt(plaintext, key);
        byte[] message = chug.Decrypt(ciphertext, key);

        Console.WriteLine("key\t\t:\t{0}", BitConverter.ToString(key).Replace("-", "").ToLower());
        Console.WriteLine("plaintext\t:\t{0}", BitConverter.ToString(plaintext).Replace("-", "").ToLower());
        Console.WriteLine("ciphertext\t:\t{0}", BitConverter.ToString(ciphertext).Replace("-", "").ToLower());
        Console.WriteLine("message\t\t:\t{0}", BitConverter.ToString(secret).Replace("-", "").ToLower());
    }
}
```

#### Expected Output

```
key             :       4d792073757065722073656372657420656e6372797074696f6e206b6579
plaintext       :       4d7920736563726574206d657373616765
ciphertext      :       c2a10df2c28a08c0c2b89913c28d2c60c2947f79c2958b58c28db24ec2947f79c28ca66ec2b89913c2904ffcc2947f79c28d2c60c28d2c60c2969737c293739bc2947f79
message         :       4d7920726563726574206d657272616765
```

## Suggestions & Bug Reports

If you would like to suggest a feature or report a bug please see the [**issues**](https://github.com/sblmnl/ChugSharp/issues) page.

## Credits

* [**Jared Shue**](https://github.com/sblmnl) - **Developer**

See the [**contributors**](https://github.com/sblmnl/ChugSharp/contributors) page for a list of all project participants.

## License

This project is licensed under the **MIT** license - see the [**LICENSE**](LICENSE) file for details.