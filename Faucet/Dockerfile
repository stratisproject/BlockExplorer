FROM microsoft/dotnet:2.0.0-sdk-jessie
WORKDIR /

# dotnet dependencies:
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash -
RUN apt-get install -y nodejs

# install stratfaucet
WORKDIR /

RUN git clone https://github.com/patrickafoley/stratfaucet

COPY appsettings.json.docker /stratfaucet/appsettings.json

RUN cd stratfaucet \
  && npm install \
  && dotnet restore \
  && dotnet publish

EXPOSE 5000
CMD cd /stratfaucet ; dotnet run
