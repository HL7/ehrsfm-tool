FROM node:lts-buster

WORKDIR /app

# fm publication tool
RUN apt update
RUN apt install -y openjdk-11-jdk-headless curl
RUN curl https://downloads.apache.org//ant/binaries/apache-ant-1.10.9-bin.tar.gz -o /tmp/apache-ant.tar.gz
RUN tar -zxvf /tmp/apache-ant.tar.gz -C /opt
ENV PATH=$PATH:/opt/apache-ant-1.10.9/bin
RUN curl https://archive.apache.org/dist/xmlgraphics/fop/binaries/fop-2.6-bin.tar.gz -o /tmp/fop.tar.gz
RUN tar -zxvf /tmp/fop.tar.gz -C /opt

# dotnet runtime and HL7_FM_CLI ConcolsApp
RUN apt install -y wget
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb
RUN dpkg -i /tmp/packages-microsoft-prod.deb
RUN apt update
RUN apt install -y dotnet-sdk-2.2

# additional for scripts
RUN npm install

RUN echo 'dotnet build HL7_FM_CLI.ConsoleApp'
CMD /bin/bash