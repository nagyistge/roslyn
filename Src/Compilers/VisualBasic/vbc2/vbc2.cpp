
#include "native_client.h"

int main(int argc, char * argv[])
{
	return Run(RequestLanguage::VBCOMPILE, L"vbc2.resources.dll");
}