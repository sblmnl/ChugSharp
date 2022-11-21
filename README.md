# ChugSharp

[![](https://img.shields.io/github/license/sblmnl/ChugSharp?color=red&style=flat-square)](LICENSE)
[![](https://img.shields.io/github/workflow/status/sblmnl/ChugSharp/.NET?style=flat-square)](https://github.com/sblmnl/ChugSharp/actions/workflows/dotnet.yml)
[![](https://img.shields.io/github/v/release/sblmnl/ChugSharp?color=lightgray&style=flat-square)](https://github.com/sblmnl/ChugSharp/releases)

The official implementation of the [Chug](https://github.com/sblmnl/chug) encryption and padding algorithms.

## Disclaimer

Chug is an enthusiast algorithm and is not intended for use in any security applications.

## Getting Started

### Sample Code

```csharp
using ChugSharp;
using System;
using System.Text;

class Program
{
    static void Main()
    {
        var chug = new Chug(usePadding: true);

        byte[] key = Encoding.UTF8.GetBytes("My super secret encryption key");
        byte[] plaintext = Encoding.UTF8.GetBytes("My secret message");
        byte[] ciphertext = chug.Encrypt(plaintext, key);
        byte[] message = chug.Decrypt(ciphertext, key);

        Console.WriteLine("key\t\t:\t{0}", BitConverter.ToString(key).Replace("-", "").ToLower());
        Console.WriteLine("plaintext\t:\t{0}", BitConverter.ToString(plaintext).Replace("-", "").ToLower());
        Console.WriteLine("ciphertext\t:\t{0}", BitConverter.ToString(ciphertext).Replace("-", "").ToLower());
        Console.WriteLine("message\t\t:\t{0}", BitConverter.ToString(message).Replace("-", "").ToLower());
    }
}
```

#### Expected Output

```
key             :       4d792073757065722073656372657420656e6372797074696f6e206b6579
plaintext       :       4d7920736563726574206d657373616765
ciphertext      :       35b9604284af02474c7668f3de29c926a0f99a3e8a4b8c4d47468c4c9a3ab517
message         :       4d7920736563726574206d657373616765
```

## Suggestions & Bug Reports

If you would like to suggest a feature or report a bug please see the [**issues**](https://github.com/sblmnl/ChugSharp/issues) page.

## Credits

* [**sblmnl**](https://github.com/sblmnl) - **Developer**

See the [**contributors**](https://github.com/sblmnl/ChugSharp/contributors) page for a list of all project participants.

## License

This project is licensed under the **MIT** license - see the [**LICENSE**](LICENSE) file for details.
