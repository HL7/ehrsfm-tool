FROM node:latest

WORKDIR /app

# fm publication tool
RUN apt update
RUN apt install -y openjdk-17-jdk-headless curl
ENV PATH=$PATH:/opt/apache-ant-1.10.14/bin

# 3. Download and extract tools in single RUN steps to avoid saving tarballs in layers
RUN curl -fSL https://archive.apache.org/dist/ant/binaries/apache-ant-1.10.14-bin.tar.gz -o /tmp/apache-ant.tar.gz \
    && tar -zxvf /tmp/apache-ant.tar.gz -C /opt \
    && rm /tmp/apache-ant.tar.gz

RUN curl -fSL https://archive.apache.org/dist/pdfbox/2.0.30/pdfbox-app-2.0.30.jar -o /opt/pdfbox-app-2.0.30.jar

# dotnet runtime and HL7_FM_CLI ConcolsApp
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O /tmp/packages-microsoft-prod.deb \
    && dpkg -i /tmp/packages-microsoft-prod.deb \
    && rm /tmp/packages-microsoft-prod.deb \
    && apt-get update \
    && apt-get install -y --no-install-recommends dotnet-sdk-8.0 \
    && rm -rf /var/lib/apt/lists/*

# additional for scripts (no package info???)
#RUN npm install

RUN echo 'dotnet build HL7_FM_CLI.ConsoleApp'
CMD /bin/bash
